package crypto

import (
	"bytes"
	"crypto/aes"
	"crypto/cipher"
	"encoding/hex"
)

// EncryptAES : 加密 AES(CBC)
func EncryptAES(data string, key string, iv string) (string, error) {
	origData := []byte(data)
	block, err := aes.NewCipher([]byte(key)) // 分組密鑰, key 長度須為16 or 24 or 32
	if err != nil {
		return "", err
	}

	blockSize := block.BlockSize()                         // 獲取密鑰塊的長度
	origData = pkcs5Padding(origData, blockSize)           // 填充長度
	blockMode := cipher.NewCBCEncrypter(block, []byte(iv)) // 加密模式
	encrypted := make([]byte, len(origData))               // 創建數組
	blockMode.CryptBlocks(encrypted, origData)             // 加密
	return hex.EncodeToString(encrypted), nil
}

// DecryptAES : 解密 AES(CBC)
func DecryptAES(data string, key string, iv string) (string, error) {
	encrypted, err := hex.DecodeString(data)
	if err != nil {
		return "", err
	}

	block, err := aes.NewCipher([]byte(key)) // 分組密鑰
	if err != nil {
		return "", err
	}

	blockMode := cipher.NewCBCDecrypter(block, []byte(iv)) // 加密模式
	decrypted := make([]byte, len(encrypted))              // 創建數組
	blockMode.CryptBlocks(decrypted, encrypted)            // 解密
	decrypted = pkcs5UnPadding(decrypted)                  // 去除填充長度
	return string(decrypted), nil
}

// pkcs5Padding : 長度補位填充
func pkcs5Padding(ciphertext []byte, blockSize int) []byte {
	padding := blockSize - len(ciphertext)%blockSize
	padtext := bytes.Repeat([]byte{byte(padding)}, padding)
	return append(ciphertext, padtext...)
}

// pkcs5UnPadding : 去除長度補位填充
func pkcs5UnPadding(origData []byte) []byte {
	length := len(origData)
	unpadding := int(origData[length-1])
	return origData[:(length - unpadding)]
}
