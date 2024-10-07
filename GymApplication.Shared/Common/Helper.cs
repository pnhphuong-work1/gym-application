namespace GymApplication.Shared.Common;

public static class Helper
{
    public static string GetEmailTemplate(string? fullName, string verificationLink)
    {
        var emailTemplate = """

                            <!DOCTYPE html>
                            <html lang='en'>
                            <head>
                                <meta charset='UTF-8'>
                                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                                <title>Verify Your Email</title>
                                <style>
                                    body {
                                        font-family: Arial, sans-serif;
                                        background-color: #f4f4f4;
                                        color: #333;
                                        margin: 0;
                                        padding: 0;
                                    }
                                    .container {
                                        width: 100%;
                                        max-width: 600px;
                                        margin: 0 auto;
                                        padding: 20px;
                                        background-color: #ffffff;
                                        border-radius: 10px;
                                        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                                    }
                                    .header {
                                        background-color: #4CAF50;
                                        color: white;
                                        padding: 10px 0;
                                        text-align: center;
                                        border-radius: 10px 10px 0 0;
                                    }
                                    .content {
                                        padding: 20px;
                                        text-align: center;
                                    }
                                    .button {
                                        background-color: #4CAF50;
                                        color: white;
                                        padding: 15px 25px;
                                        text-decoration: none;
                                        border-radius: 5px;
                                        display: inline-block;
                                        margin-top: 20px;
                                    }
                                    .footer {
                                        margin-top: 20px;
                                        font-size: 12px;
                                        color: #777;
                                        text-align: center;
                                    }
                                </style>
                            </head>
                            <body>
                                <div class='container'>
                                    <div class='header'>
                                        <h1>Verify Your Email</h1>
                                    </div>
                                    <div class='content'>
                                        <p>Hi <strong>{{FullName}}</strong>,</p>
                                        <p>Thank you for signing up! Please verify your email address to complete your registration.</p>
                                        <a href='{{VerificationLink}}' class='button'>Verify Email</a>
                                    </div>
                                    <div class='footer'>
                                        <p>If you did not sign up, you can safely ignore this email.</p>
                                    </div>
                                </div>
                            </body>
                            </html>
                            """;

        // Replace placeholders with actual values
        emailTemplate = emailTemplate.Replace("{{FullName}}", fullName);
        emailTemplate = emailTemplate.Replace("{{VerificationLink}}", verificationLink);

        return emailTemplate;
    }

    public static string GetForgotPasswordEmailTemplate(string? fullName, string resetLink)
    {
        var emailTemplate = """
                            <!DOCTYPE html>
                            <html lang='en'>
                            <head>
                                <meta charset='UTF-8'>
                                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                                <title>Reset Your Password</title>
                                <style>
                                    body {
                                        font-family: Arial, sans-serif;
                                        background-color: #f4f4f4;
                                        color: #333;
                                        margin: 0;
                                        padding: 0;
                                    }
                                    .container {
                                        width: 100%;
                                        max-width: 600px;
                                        margin: 0 auto;
                                        padding: 20px;
                                        background-color: #ffffff;
                                        border-radius: 10px;
                                        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                                    }
                                    .header {
                                        background-color: #f44336;
                                        color: white;
                                        padding: 10px 0;
                                        text-align: center;
                                        border-radius: 10px 10px 0 0;
                                    }
                                    .content {
                                        padding: 20px;
                                        text-align: center;
                                    }
                                    .button {
                                        background-color: #f44336;
                                        color: white;
                                        padding: 15px 25px;
                                        text-decoration: none;
                                        border-radius: 5px;
                                        display: inline-block;
                                        margin-top: 20px;
                                    }
                                    .footer {
                                        margin-top: 20px;
                                        font-size: 12px;
                                        color: #777;
                                        text-align: center;
                                    }
                                </style>
                            </head>
                            <body>
                                <div class='container'>
                                    <div class='header'>
                                        <h1>Reset Your Password</h1>
                                    </div>
                                    <div class='content'>
                                        <p>Hi <strong>{{FullName}}</strong>,</p>
                                        <p>We received a request to reset your password. Click the button below to reset your password:</p>
                                        <a href='{{ResetLink}}' class='button'>Reset Password</a>
                                        <p>If you did not request this, please ignore this email or contact support.</p>
                                    </div>
                                    <div class='footer'>
                                        <p>This password reset link will expire in 24 hours.</p>
                                    </div>
                                </div>
                            </body>
                            </html>
                            """;

        // Replace placeholders with actual values
        emailTemplate = emailTemplate.Replace("{{FullName}}", fullName);
        emailTemplate = emailTemplate.Replace("{{ResetLink}}", resetLink);

        return emailTemplate;
    }
}