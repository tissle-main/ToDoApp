module.exports = {
  "/api": {
    target: process.env["services__todoapp-web__https__0"] || process.env["services__todoapp-web_http__0"],
    secure: process.env["NODE_ENV"] !== "development",
    pathRewrite: {
      "^/api": ""
    }
  }
}
