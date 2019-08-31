<template>
  <el-container id="main-view">
    <el-header class="header">
          <top-bar :code="loginName" @change_name="changeName"></top-bar>
    </el-header>
    <el-main>
        这是正文部分,欢迎： {{loginName}}
    </el-main>
  </el-container>
</template>

<script>
import TopBar from "@/components/top-bar.vue";
export default {
    name: "home",
    data(){
          return {
            loginName: this.$route.query.code
        };
    },
    components:{
         TopBar
    },
    methods:{
        changeName(name){
            this.loginName=name;
        }
    }
};
</script>
<style lang="css" scoped>
.header{
    background-color: rgb(0, 120, 215);
    color:aliceblue;
    padding-left:0;
    height:48px;
}
</style>
