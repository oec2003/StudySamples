import http from '@/common/http'

const LoginService = {

    login: async function(code, password) {
        let res = await http.POST('/api/Login', {
            Code: code || '',
            Pwd: password || ''
        })
        if (res.Success && res.Result.Result) {
            // 登录成功后的处理

        } else {
            res.Success = false;
            res.Message = res.Result.Message;
        }
        return res;
    }
}
export default LoginService