package database

import (
	"reflect"
	"strings"
)

// SQLColumn : 取得結構體 db 欄位名稱
func SQLColumn(target interface{}, sqlTag string) ([]string, string, map[string]string) {
	var fields []string
	var fieldMap map[string]string = map[string]string{}
	el := reflect.TypeOf(target).Elem()
	for i := 0; i < el.NumField(); i++ {
		field := el.Field(i)
		if sqlTag == "" {
			fields = append(fields, field.Name)
			fieldMap[field.Name] = field.Name
		} else {
			tagName := field.Tag.Get(sqlTag)
			if tagName != "" {
				fields = append(fields, tagName)
				fieldMap[field.Name] = tagName
			}
		}
	}

	return fields, strings.Join(fields, ","), fieldMap
}

// SQLColumnValue : 取得結構體 db 欄位資料位址
func SQLColumnValue(target interface{}, sqlTag string) []interface{} {
	var sqlValues []interface{}
	rv := reflect.ValueOf(target).Elem()
	el := reflect.TypeOf(target).Elem()
	for i := 0; i < el.NumField(); i++ {
		field := el.Field(i)
		fieldName := field.Name
		if sqlTag != "" {
			fieldName = field.Tag.Get(sqlTag)
		}

		if fieldName != "" {
			sqlValues = append(sqlValues, rv.Field(i).Addr().Interface())
		}
	}

	return sqlValues
}
