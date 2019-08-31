<template>
    <div>这是网站的头部, 欢迎：{{code}}  
    <el-button type="primary" @click="changeName">改变名称</el-button></div>
</template>
<script>
export default {
    name:"top-bar",
    props: {
        code: String
    },
    methods:{
        changeName(){
            return this.$emit("change_name","oec2003");
        }
    }
}
</script>
<style scoped>
</style>