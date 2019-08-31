<template>
  <el-form ref="ruleForm" :model="formModel">
    <p class="title">登录</p>
    <el-form-item required>
      <el-input v-model="formModel.loginName" placeholder="请输入登录名" size="large"></el-input>
    </el-form-item>
    <el-form-item required>
      <el-input
        type="password"
        v-model="formModel.password"
        @keyup.native="keyup"
        placeholder="请输入登录密码"
        size="large"
      ></el-input>
    </el-form-item>
    <p v-show="showErrorMessage" class="alert">{{ errorMessage }}</p>
    <el-form-item>
      <el-button type="primary" size="medium" @click="login" style="width: 100%;">登录</el-button>
    </el-form-item>
  </el-form>
</template>
<script>
import LoginService from '@/services/login.service'
export default {
  name: "login",
  data() {
    return {
      errorMessage: "",
      formModel: {
        loginName: "",
        password: ""
      }
    };
  },
  watch: {
    "formModel.password": function(val) {
      if (val.trim() !== "") {
        this.errorMessage = "";
      } else {
        this.errorMessage = "请输入密码";
      }
    }
  },
  computed: {
    showErrorMessage() {
      return this.errorMessage !== "";
    }
  },
  mounted: function() {},
  methods: {
    keyup(event) {
      if (event.keyCode === 13) {
        this.login();
      }
    },

    login() {
      const loginName = this.formModel.loginName;
      const password = this.formModel.password;
      if (loginName === "") {
        this.errorMessage = "请输入登录名";
        return;
      }
      if (password === "") {
        this.errorMessage = "请输入密码";
        return;
      }
      //调用接口验证
      LoginService.login(loginName, password).then(res => {
        if (res.Success) {
          this.$router.push({
            path: "/",
            query: {
              code: loginName
            }
          });
        } else {
          this.errorMessage = res.Message;
        }
      });

      //   this.$router.push({
      //     path: "/",
      //     query: {
      //         code: loginName
      //       }
      //   });
    }
  }
};
</script>
<style scoped>
</style>