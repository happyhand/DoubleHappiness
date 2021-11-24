package jwtservice

import (
	"time"

	"github.com/dgrijalva/jwt-go"
	"github.com/google/uuid"
)

const (
	/**
	* jwt claims value
	**/

	audience  string        = "GoBike-Server"
	issuer    string        = "Jiale"
	subject   string        = "GoBike-Client"
	expiresAt time.Duration = 300

	// PayloadKey : jwt claims payload flag of api request
	PayloadKey string = "user"
)

// Claims : jwt claims
type Claims struct {
	Payload Payload `json:"payload"`
	jwt.StandardClaims
}

// Payload : jwt claims payload
type Payload struct {
	Avatar     string
	Email      string
	FrontCover string
	MemberID   string
	Nickname   string
	Photo      string
}

// jwtSecret : jwt secret key
var jwtSecret = []byte(uuid.NewString())
