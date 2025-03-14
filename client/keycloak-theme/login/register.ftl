<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <title>Register - Coin Hawk</title>
    <link rel="stylesheet" type="text/css" href="${url.resourcesPath}/css/style.css">
</head>
<body>
    <div class="login-page">
      <div class="left-section">
        <div class="welcome-text">
          <h1>Create Account</h1>
          <p>Join Coin Hawk for the ultimate cryptocurrency experience.</p>
        </div>
        <#-- Registration form -->
        <form action="${url.registrationAction}" method="post" id="kc-form-register" class="registration-form">
          <input type="hidden" name="tab_id" value="${tabId!''}">
          <input type="hidden" name="client_id" value="${clientId!''}">
          <div>
            <input type="text" placeholder="Username" class="input-field" name="username" value="${username!''}" required autofocus />
          </div>
          <div>
            <input type="email" placeholder="Email" class="input-field" name="email" value="${email!''}" required />
          </div>
          <div>
            <input type="text" placeholder="First Name" class="input-field" name="firstName" value="${firstName!''}" />
          </div>
          <div>
            <input type="text" placeholder="Last Name" class="input-field" name="lastName" value="${lastName!''}" />
          </div>
          <div>
            <input type="password" placeholder="Password" class="input-field" name="password" required />
          </div>
          <div>
            <input type="password" placeholder="Confirm Password" class="input-field" name="password-confirm" required />
          </div>
          <div class="button-container">
            <button class="btn" type="submit">Register</button>
          </div>
        </form>
        <p style="text-align: center; margin-top: 15px;">
          <a href="${url.loginUrl}">Already have an account? Login</a>
        </p>
      </div>
      <div class="right-section">
        <img src="https://static.coingecko.com/s/coingecko-logo-8903d34ce19ca4be1c81f0db30e924154750d208683fad7ae6f2ce06c76d0a56.png" alt="Coin Logo" class="coin-image" />
      </div>
    </div>
</body>
</html>
