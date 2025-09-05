# BondTales Chat Backend - Render Deployment Guide

This guide will walk you through deploying your .NET 8 BondTales Chat backend to Render.

## Prerequisites

1. **GitHub Repository**: Your code should be in a GitHub repository
2. **Render Account**: Sign up at [render.com](https://render.com)
3. **Environment Variables**: Prepare your environment variables (see below)

## Step 1: Prepare Your Repository

### 1.1 Commit the Deployment Files
Make sure these files are committed to your repository:
- `render.yaml` (deployment configuration)
- `Dockerfile` (container configuration)
- All your source code

```bash
git add .
git commit -m "Add Render deployment configuration"
git push origin main
```

### 1.2 Update CORS Settings
In `Program.cs`, replace the placeholder domains with your actual frontend domains:
```csharp
.WithOrigins(
    "http://localhost:4200",
    "https://localhost:4200",
    "https://your-actual-frontend-domain.onrender.com", // Replace this
    "https://your-actual-frontend-domain.vercel.app",   // Replace this
    "https://your-actual-frontend-domain.netlify.app"   // Replace this
)
```

## Step 2: Deploy to Render

### 2.1 Create a New Web Service
1. Go to [render.com](https://render.com) and sign in
2. Click **"New +"** → **"Web Service"**
3. Connect your GitHub repository
4. Select your repository: `BondTalesChat-server`

### 2.2 Configure the Service
1. **Name**: `bondtaleschat-backend`
2. **Environment**: `Docker`
3. **Region**: Choose the closest to your users
4. **Branch**: `main` (or your default branch)
5. **Root Directory**: `BondTalesChat-Server` (the folder containing your .csproj file)

### 2.3 Set Environment Variables
Add these environment variables in the Render dashboard:

#### Required Variables:
```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:$PORT
```

#### Database Connection:
```
ConnectionStrings__DefaultConnection=<Your PostgreSQL connection string>
```

#### JWT Configuration:
```
Jwt__Key=KoushiikMuralitharanAndAkashSettukannu
Jwt__Issuer=BondTalesChat
Jwt__Audience=BondTalesChat
Jwt__ExpiresInMinutes=60
```

#### SMTP Configuration:
```
Smtp__FromEmail=koushiik2003@gmail.com
Smtp__FromName=BondTales Chat Application Support
Smtp__Host=smtp.gmail.com
Smtp__Port=587
Smtp__EnableSsl=true
Smtp__Username=<Your Gmail username>
Smtp__Password=<Your Gmail app password>
```

### 2.4 Create PostgreSQL Database
1. In Render dashboard, click **"New +"** → **"PostgreSQL"**
2. **Name**: `bondtaleschat-db`
3. **Database**: `bondtaleschat`
4. **User**: `bondtaleschat_user`
5. **Plan**: Choose based on your needs (Free tier available)

### 2.5 Update Database Connection
1. Copy the connection string from your PostgreSQL service
2. Update the `ConnectionStrings__DefaultConnection` environment variable in your web service

## Step 3: Alternative Deployment (Using render.yaml)

If you prefer using the `render.yaml` file:

### 3.1 Blueprint Deployment
1. In Render dashboard, click **"New +"** → **"Blueprint"**
2. Connect your GitHub repository
3. Select the `render.yaml` file
4. Render will automatically create both the web service and database

### 3.2 Manual Configuration
If using the YAML file, you'll still need to:
1. Set the SMTP credentials manually in the web service environment variables
2. Update CORS settings with your actual frontend domains

## Step 4: Configure Custom Domain (Optional)

1. In your web service settings, go to **"Custom Domains"**
2. Add your custom domain
3. Follow the DNS configuration instructions

## Step 5: Test Your Deployment

### 5.1 Health Check
Your service should be accessible at:
- `https://your-service-name.onrender.com/swagger` (Swagger UI)
- `https://your-service-name.onrender.com/chatHub` (SignalR Hub)

### 5.2 API Endpoints
Test your API endpoints:
- `GET https://your-service-name.onrender.com/api/weatherforecast`
- `POST https://your-service-name.onrender.com/api/user/register`

## Step 6: Update Frontend Configuration

Update your frontend application to use the new backend URL:
```typescript
// In your frontend environment configuration
export const environment = {
  production: true,
  apiUrl: 'https://your-service-name.onrender.com',
  signalRUrl: 'https://your-service-name.onrender.com/chatHub'
};
```

## Troubleshooting

### Common Issues:

1. **Build Failures**:
   - Check that all NuGet packages are properly referenced
   - Ensure the project builds locally

2. **Database Connection Issues**:
   - Verify the connection string format
   - Check that the database is accessible from Render

3. **CORS Errors**:
   - Update CORS settings with your actual frontend domain
   - Ensure credentials are properly configured

4. **SignalR Connection Issues**:
   - Check that the hub URL is correct
   - Verify WebSocket support is enabled

### Logs and Monitoring:
- Check the **"Logs"** tab in your Render service for detailed error messages
- Monitor the **"Metrics"** tab for performance insights

## Environment Variables Reference

| Variable | Description | Example |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Environment name | `Production` |
| `ASPNETCORE_URLS` | Server binding URL | `http://0.0.0.0:$PORT` |
| `ConnectionStrings__DefaultConnection` | Database connection | `Host=...;Database=...;Username=...;Password=...` |
| `Jwt__Key` | JWT signing key | `YourSecretKey` |
| `Jwt__Issuer` | JWT issuer | `BondTalesChat` |
| `Jwt__Audience` | JWT audience | `BondTalesChat` |
| `Jwt__ExpiresInMinutes` | Token expiration | `60` |
| `Smtp__Username` | Email username | `your-email@gmail.com` |
| `Smtp__Password` | Email app password | `your-app-password` |

## Security Notes

1. **Never commit sensitive data** like passwords or API keys to your repository
2. **Use environment variables** for all sensitive configuration
3. **Enable HTTPS** for production (Render provides this automatically)
4. **Regularly update dependencies** to patch security vulnerabilities

## Cost Optimization

1. **Free Tier**: Render offers a free tier with limitations
2. **Auto-sleep**: Free services sleep after 15 minutes of inactivity
3. **Upgrade**: Consider upgrading for production use with higher traffic

## Support

- **Render Documentation**: [render.com/docs](https://render.com/docs)
- **Render Community**: [community.render.com](https://community.render.com)
- **Your Service Logs**: Check the Logs tab in your Render dashboard

---

**Note**: Remember to replace placeholder values (like `your-service-name` and `your-frontend-domain`) with your actual values throughout this guide.
