import Vue from "vue";
import Router from "vue-router";
Vue.use(Router);
const router = new Router({
    mode: 'history',
    routes: [{
            path: "/",
            component: () =>
                import ("@/components/home.vue")
        }, {
            path: "/login",
            name: "login",
            component: () =>
                import ("@/components/login.vue")
        },
        {
            path: "*",
            redirect: "/"
        }
    ]
})
export default router;