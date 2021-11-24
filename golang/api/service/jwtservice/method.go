package jwtservice

import (
	"time"

	"api/core/customerror"

	"github.com/dgrijalva/jwt-go"
	"github.com/google/uuid"
)

// Generate : 生成 token
func Generate(payload Payload) (string, *customerror.Error) {
	var utc time.Time = time.Now().UTC()
	var claims Claims
	claims.Payload = payload
	claims.Audience = audience
	claims.ExpiresAt = utc.Add(time.Minute * expiresAt).Unix()
	claims.Id = uuid.NewString()
	claims.IssuedAt = utc.Unix()
	claims.Issuer = issuer
	claims.NotBefore = utc.Unix()
	claims.Subject = subject
	token := jwt.NewWithClaims(jwt.SigningMethodHS256, claims)
	str, err := token.SignedString(jwtSecret)
	if err != nil {
		return "", customerror.New(customerror.SystemError, err.Error())
	}

	return str, nil
}

// Validate : 驗證 token
func Validate(token string) (*Payload, *customerror.Error) {
	var claims Claims
	jwtToken, err := jwt.ParseWithClaims(token, &claims, func(t *jwt.Token) (interface{}, error) {
		return jwtSecret, nil
	})

	if err != nil {
		return nil, customerror.New(customerror.Unauthorized, err.Error())
	}

	if !jwtToken.Valid {
		return nil, customerror.New(customerror.Forbidden, "illegal jwt token")
	}

	if claims.Audience != audience {
		return nil, customerror.New(customerror.Forbidden, "illegal audience")
	}

	if claims.Issuer != issuer {
		return nil, customerror.New(customerror.Forbidden, "illegal issuer")
	}

	return &claims.Payload, nil
}
