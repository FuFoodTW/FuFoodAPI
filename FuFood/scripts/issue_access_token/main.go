package main

import (
	"crypto/hkdf"
	"crypto/sha256"
	"database/sql"
	"encoding/base64"
	"encoding/json"
	"errors"
	"fmt"
	"log"
	"net/url"
	"os"
	"path/filepath"
	"strings"
	"time"

	"github.com/golang-jwt/jwt/v5"
	_ "github.com/lib/pq"
)

func MustGetenv(name string) string {
	val := os.Getenv(name)
	if val == "" {
		log.Fatalf("Environment variable %s is not set!", name)
	}
	return val
}

func MustGetenvBase64(name string) []byte {
	val := MustGetenv(name)
	bytes, err := base64.StdEncoding.DecodeString(val)
	if err != nil {
		log.Fatalf("Failed to parse environment variable %s from Base64:", err)
	}
	return bytes
}

const Salt = "富食品版權所有,不要再猜我們的秘密值"
const Info = "access-token-signer"

const SettingsFile = "appsettings.Development.json"

const JoLineID = "U78d1bb86f8410a52d375306295c06503"

type AppSettings struct {
	ConnectionStrings struct {
		AppDbContext string `json:"AppDbContext"`
	} `json:"ConnectionStrings"`
	Crypto struct {
		SecretKeyBase []byte `json:"SecretKeyBase"`
	} `json:"Crypto"`
}

func FindAppSettingsFile() (string, error) {
	// Start from current working directory
	currentDir, err := os.Getwd()
	if err != nil {
		return "", fmt.Errorf("failed to get current directory: %w", err)
	}

	// Traverse up the directory tree
	for {
		// Check for SettingsFile in current directory
		appSettingsPath := filepath.Join(currentDir, SettingsFile)

		if _, err := os.Stat(appSettingsPath); err == nil {
			// File exists
			return appSettingsPath, nil
		}

		// Get parent directory
		parentDir := filepath.Dir(currentDir)

		// Check if we've reached the root
		if parentDir == currentDir {
			return "", fmt.Errorf("%s not found in any parent directory", SettingsFile)
		}

		currentDir = parentDir
	}
}

func LoadAppSettings() (*AppSettings, error) {
	// Find the file
	filePath, err := FindAppSettingsFile()
	if err != nil {
		return nil, err
	}

	// Read the file
	data, err := os.ReadFile(filePath)
	if err != nil {
		return nil, fmt.Errorf("failed to read %s: %w", SettingsFile, err)
	}

	// Parse JSON
	var settings AppSettings
	if err := json.Unmarshal(data, &settings); err != nil {
		return nil, fmt.Errorf("failed to parse %s: %w", SettingsFile, err)
	}

	return &settings, nil
}

// ParseEFCoreConnectionString converts an EF Core connection string to a database/sql URL
func ParseEFCoreConnectionString(connStr string) (string, error) {
	params := make(map[string]string)

	// Split by semicolon and parse key=value pairs
	pairs := strings.Split(connStr, ";")
	for _, pair := range pairs {
		pair = strings.TrimSpace(pair)
		if pair == "" {
			continue
		}

		parts := strings.SplitN(pair, "=", 2)
		if len(parts) != 2 {
			continue
		}

		key := strings.TrimSpace(parts[0])
		value := strings.TrimSpace(parts[1])
		params[strings.ToLower(key)] = value
	}

	// Extract common parameters (case-insensitive)
	host := getParam(params, "host", "server", "data source")
	port := getParam(params, "port")
	database := getParam(params, "database", "initial catalog")
	username := getParam(params, "username", "user id", "uid", "user")
	password := getParam(params, "password", "pwd")

	// Default port if not specified
	if port == "" {
		port = "5432"
	}

	// Validate required fields
	if host == "" {
		return "", fmt.Errorf("host not found in connection string")
	}
	if database == "" {
		return "", fmt.Errorf("database not found in connection string")
	}

	// Build PostgreSQL URL
	var pgURL string
	if username != "" && password != "" {
		pgURL = fmt.Sprintf("postgres://%s:%s@%s:%s/%s",
			url.QueryEscape(username),
			url.QueryEscape(password),
			host,
			port,
			database,
		)
	} else if username != "" {
		pgURL = fmt.Sprintf("postgres://%s@%s:%s/%s",
			url.QueryEscape(username),
			host,
			port,
			database,
		)
	} else {
		pgURL = fmt.Sprintf("postgres://%s:%s/%s",
			host,
			port,
			database,
		)
	}

	// Add optional parameters (SSL mode, etc.)
	queryParams := url.Values{}

	if sslMode := getParam(params, "ssl mode", "sslmode"); sslMode != "" {
		queryParams.Add("sslmode", sslMode)
	}

	if timeout := getParam(params, "timeout", "command timeout"); timeout != "" {
		queryParams.Add("connect_timeout", timeout)
	}

	if len(queryParams) > 0 {
		pgURL += "?" + queryParams.Encode()
	}

	return pgURL, nil
}

// getParam retrieves a parameter by checking multiple possible key names (case-insensitive)
func getParam(params map[string]string, keys ...string) string {
	for _, key := range keys {
		if value, ok := params[strings.ToLower(key)]; ok {
			return value
		}
	}
	return ""
}

func main() {
	settings, err := LoadAppSettings()
	if err != nil {
		log.Fatal(err)
	}

	signer, err := hkdf.Key(sha256.New, settings.Crypto.SecretKeyBase, []byte(Salt), Info, 32)
	if err != nil {
		log.Fatal(err)
	}
	_ = signer

	connStr, err := ParseEFCoreConnectionString(settings.ConnectionStrings.AppDbContext)
	if err != nil {
		log.Fatal(err)
	}

	conn, err := sql.Open("postgres", connStr+"?sslmode=disable")
	if err != nil {
		log.Fatal(err)
	}

	requestedId := JoLineID
	if len(os.Args) > 1 {
		requestedId = os.Args[1]
	}

	var userId string
	err = conn.QueryRow(`select "Id"::text from "Users" where "LineId" = $1 or "Id"::text = $1`, requestedId).Scan(&userId)
	if err != nil && errors.Is(err, sql.ErrNoRows) {
		log.Fatalf(`No user with Line ID %s found in the database. Please run "dotnet run -- seed" first.`, JoLineID)
	}

	now := time.Now().Unix()
	exp := now + 86400

	token := jwt.NewWithClaims(jwt.SigningMethodHS256, jwt.MapClaims{
		"sub": userId,
		"exp": exp,
		"iat": now,
	})
	tokenString, err := token.SignedString(signer)
	if err != nil {
		log.Fatal(err)
	}

	fmt.Print(tokenString)
}
