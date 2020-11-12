const path = require("path");
const resolve = (dir) => path.join(__dirname, dir);
//压缩代码并去掉console (取消，vue-cli 3 以內建 TerserPlugin)
// const TerserPlugin = require("terser-webpack-plugin");
//代码打包zip
const CompressionWebpackPlugin = require("compression-webpack-plugin");
const productionGzipExtensions = /\.(js|css|json|txt|html|ico|svg)(\?.*)?$/i;

module.exports = {
  // 废弃baseUrl  一般运维会配置好的
  publicPath: process.env.NODE_ENV === "production" ? "/mgtgobike/" : "/",
  //打包的输出目录
  outputDir: "dist/",
  //如果文件等设置
  pages: {
    index: {
      entry: "src/main.ts",
      template: "public/index.html",
      filename: "index.html",
    },
  },
  //静态资源打包路径
  assetsDir: "static",
  //默认false 可以加快打包
  productionSourceMap: false,
  //打包后的启动文件
  indexPath: "index.html",
  //打包的css路径及命名
  css: {
    requireModuleExtension: true,
    //vue 文件中修改css 不生效 注释掉  extract:true
    extract: {
      filename: "style/[name].[hash:8].css",
      chunkFilename: "style/[name].[hash:8].css",
    },
    sourceMap: false,
  },
  //webpack 链式配置   默认已经配置好了  node_moudles/@vue
  chainWebpack: (config) => {
    // 修复HMR
    config.resolve.symlinks(true);
    // 修复Lazy loading routes  按需加载的问题，如果没有配置按需加载不需要写，会报错
    // config.plugin("html").tap(args => {
    //   args[0].chunksSortMode = "none";
    //   return args;
    // });
    //添加别名
    config.resolve.alias
      .set("@", resolve("src"))
      .set("assets", resolve("src/assets"))
      .set("components", resolve("src/components"))
      .set("layout", resolve("src/layout"))
      .set("base", resolve("src/base"))
      .set("static", resolve("src/static"));
    // 压缩图片
    config.module
      .rule("images")
      .use("image-webpack-loader")
      .loader("image-webpack-loader")
      .options({
        mozjpeg: { progressive: true, quality: 65 },
        optipng: { enabled: false },
        pngquant: { quality: [0.65, 0.9], speed: 4 },
        gifsicle: { interlaced: false },
        webp: { quality: 75 },
      });
  },
  //webpack 配置
  configureWebpack: (config) => {
    //#region TODO 此為暫解，待解決如何修改 TerserPlugin 中的參數
    const terserPlugin = config.optimization.minimizer.find(
      (data) => data.constructor.name === "TerserPlugin"
    );
    if (terserPlugin) {
      const compress = terserPlugin.options.terserOptions.compress;
      compress["drop_console"] = true;
      compress["drop_debugger"] = false;
      compress["pure_funcs"] = ["console.log"];
    }
    //#endregion

    const plugins = [];
    //启用代码压缩 (取消，vue-cli 3 以內建 TerserPlugin)
    // plugins.push(
    //   new TerserPlugin({
    //     parallel: true,
    //     terserOptions: {
    //       compress: {
    //         drop_console: true,
    //         drop_debugger: false,
    //         pure_funcs: ["console.log"], //移除console
    //       },
    //     },
    //   })
    // );
    //代码压缩打包
    plugins.push(
      new CompressionWebpackPlugin({
        filename: "[path].gz[query]",
        algorithm: "gzip",
        test: productionGzipExtensions,
        threshold: 10240,
        minRatio: 0.8,
      })
    );
    config.plugins = [...config.plugins, ...plugins];
  },
  parallel: require("os").cpus().length > 1,
  pluginOptions: {},
  pwa: {},
  //设置代理 (如果你的前端应用和后端 API 服务器没有运行在同一个主机上，你需要在开发环境下将 API 请求代理到 API 服务器)
  // https://cli.vuejs.org/zh/config/#devserver-proxy
  devServer: {
    port: 8080,
    host: "0.0.0.0",
    https: false,
    open: true,
    // openPage: "about", /// 指定預設開啟頁面
    openPage: "/", /// 指定預設開啟頁面 ex:openPage: "about"
    hot: true,
    disableHostCheck: true,
    proxy: {
      "/api": {
        target: "http://apigobike.ddns.net:18596",
        ws: true,
        changeOrigin: true,
      },
      "/foo": {
        target: "http://apigobike.ddns.net:18596",
        ws: true,
        changeOrigin: true,
      },
    },
  },
};
