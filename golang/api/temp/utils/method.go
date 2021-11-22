package utils

import "reflect"

// TransformInterfaceSlice : 轉換 interface slice
func TransformInterfaceSlice(data interface{}) []interface{} {
	var slice []interface{}
	rv := reflect.ValueOf(data)
	switch rv.Kind() {
	case reflect.Slice, reflect.Array:
		for index := 0; index < rv.Len(); index++ {
			slice = append(slice, rv.Index(index).Interface())
		}
	default:
		slice = append(slice, data)
	}

	return slice
}
