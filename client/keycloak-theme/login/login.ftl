<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <title>Login - Coin Hawk</title>
    <link rel="stylesheet" type="text/css" href="${url.resourcesPath}/css/style.css">
</head>
<body>
    <div class="login-page">
      <div class="left-section">
        <div class="welcome-text">
          <h1>Welcome back!</h1>
          <p>Coin hawk your ultimate cryptocurrency companion.</p>
        </div>
        <#-- Keycloak login form -->
        <form action="${url.loginAction}" method="post" id="kc-form-login">
          <input type="hidden" name="session_code" value="${sessionCode}">
          <input type="hidden" name="execution" value="${execution}">
          <input type="hidden" name="client_id" value="${clientId}">
          <input type="hidden" name="tab_id" value="${tabId}">

          <input type="email" placeholder="Email" class="input-field" name="username" value="${username!}" autofocus />
          <input type="password" placeholder="Password" class="input-field" name="password"/>
          <a href="#" class="forgot-password">${msg("doForgotPassword")?html}</a>
          <div class="button-container">
            <button class="btn" type="submit">Connexion</button>
            <button class="btn" type="button" onclick="document.location.href='${url.registrationUrl}'">
              Register
            </button>
          </div>
        </form>
      </div>
      <div class="right-section">
        <img src="https://static.coingecko.com/s/coingecko-logo-8903d34ce19ca4be1c81f0db30e924154750d208683fad7ae6f2ce06c76d0a56.png" alt="Coin Logo" class="coin-image" />
      </div>
    </div>
</body>
</html>
