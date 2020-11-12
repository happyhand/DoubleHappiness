export interface ApiResponse {
  content: any;
  result: boolean;
  resultCode: number;
  resultMessage: string;
}

/// typescript-eslint 規則:不需要加上 string 型別，直接使用即可
/// https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-inferrable-types.md
export const domain = "http://apigobike.ddns.net:18596";
export const singInUrl = "api/Member/Login";
