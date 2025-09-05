# Environment Variables Setup Guide

## 📋 Environment Variables File Created

I've created `environment-variables.env` with all the required environment variables for your BondTales Chat backend.

## 🚀 How to Use in Render

### **Method 1: Manual Entry (Recommended)**
1. Go to your **web service** in Render dashboard
2. Click **"Environment"** tab
3. Add each variable manually:

```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:$PORT
ConnectionStrings__DefaultConnection=Host=dpg-d2t6tb15pdvs7398r450-a.oregon-postgres.render.com;Port=5432;Database=akash_db_a9b1;Username=akash_db_a9b1_user;Password=HRgyvuUDUq4glngo22lOUiM9VOcQ1Vj8;SSL Mode=Require;
Jwt__Key=KoushiikMuralitharanAndAkashSettukannu
Jwt__Issuer=BondTalesChat
Jwt__Audience=BondTalesChat
Jwt__ExpiresInMinutes=60
Smtp__FromEmail=akashkce123@gmail.com
Smtp__FromName=BondTales Chat Application Support
Smtp__Host=smtp.gmail.com
Smtp__Port=587
Smtp__EnableSsl=true
Smtp__Username=akashkce123@gmail.com
Smtp__Password=Kce@2025
```

### **Method 2: Import from File**
1. Copy the contents of `environment-variables.env`
2. In Render dashboard, go to **"Environment"** tab
3. Look for **"Import from file"** or **"Bulk import"** option
4. Paste the contents

## ⚠️ Important Notes

1. **Email Password**: You're using your regular Gmail password. For better security, use an **App Password**:
   - Go to Google Account → Security → 2-Step Verification → App passwords
   - Generate a new app password for "Mail"
   - Replace `Kce@2025` with the generated app password

2. **Security**: These credentials are now in your repository. Consider rotating them after deployment.

3. **Database**: The connection string uses your existing database credentials.

## 🔧 Quick Setup Steps

1. **Copy the environment variables** from the file
2. **Go to Render dashboard** → Your web service → Environment
3. **Add each variable** one by one
4. **Click "Save Changes"**
5. **Render will auto-redeploy**

## 📧 Email Configuration

Your email is configured as:
- **From**: akashkce123@gmail.com
- **Password**: Kce@2025
- **SMTP**: smtp.gmail.com:587

**For production, use an App Password instead of your regular password!**

## ✅ After Setup

Once you add these environment variables:
1. Your app will connect to the database
2. JWT authentication will work
3. Email functionality will be available
4. All features should work properly

The app should deploy successfully and be fully functional!
