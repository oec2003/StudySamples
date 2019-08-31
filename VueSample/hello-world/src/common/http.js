import axios from 'axios';
axios.defaults.headers.post['Content-Type'] = 'application/json';
axios.defaults.baseURL = "";
export default {
    async GET(url, responseType) {
        try {
            const res = await axios.get(url, {
                responseType: responseType ? responseType : 'json',
                params: {
                    random: new Date().getTime()
                }
            });
            return successHandle(res);
        } catch (error) {
            return errorHandle(error);
        }
    },
    async POST(url, parameters, responseType) {
        try {
            const res = await axios.post(url, parameters, {
                responseType: responseType ? responseType : 'json',
            });
            if (responseType === "blob") {
                return res;
            } else {
                return successHandle(res);
            }
        } catch (error) {
            return errorHandle(error);
        }
    },
}

function successHandle(res) {

    const data = res.data;
    if (data.hasOwnProperty('Code')) {
        data.Success = (data.Code === HttpCode.Success && data.Status === HttpStatus.Success);
    }
    return data;
}

function errorHandle(error) {
    let meesage = '后台异常';
    if (error && error.request) {
        const status = error.request.status;
        if (status == 0) {
            meesage = '未请求到网络连接';
        }
    }
    return {
        Success: false,
        Message: meesage
    }
}

const HttpCode = {
    Success: 200,
    ValidateFailed: 203,
    Error: 500,
};
const HttpStatus = {
    Fail: 0,
    Success: 1,
    ConfirmIsContinue: 2,
    Error: 3
};