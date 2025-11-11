# API Endpoints Documentation

## Authentication
All endpoints except public ones require JWT Bearer token in Authorization header.

### Auth Endpoints
- `POST /api/auth/login` - Login
  - Request: `{ email: string, password: string }`
  - Response: `{ userId, userName, userEmail, role, token }`
  
- `GET /api/auth/profile` - Get current user profile
  - **Requires**: Authentication
  - Response: `{ accountId, accountName, accountEmail, accountRole }`

## Account Management (Admin Only - Role 0)
- `GET /api/account-management` - Get all accounts
- `GET /api/account-management/{id}` - Get account detail
- `POST /api/account-management` - Create new account
- `PUT /api/account-management/{id}` - Update account
- `DELETE /api/account-management/{id}` - Delete account

## Categories (Staff Only - Role 1)
- `GET /api/categories` - Get all categories (Public)
- `GET /api/categories/{id}` - Get category detail (Public)
- `POST /api/categories` - Create category (**Staff only**)
- `PUT /api/categories/{id}` - Update category (**Staff only**)
- `DELETE /api/categories/{id}` - Delete category (**Staff only**)

## News Articles
### Public Endpoints
- `GET /api/news-articles` - Get all news articles (No auth required)
  - Returns: List of all active news articles with category and tag information
  - Response includes: newsArticleId, newsTitle, headline, createdDate, newsContent, categoryName, tags

- `GET /api/news-articles/search?searchTerm={term}` - Search news articles (No auth required)
  - Query parameter: `searchTerm` - Search in title, headline, content, source
  - Returns: Filtered list of news articles matching the search term

- `GET /api/news-articles/{id}` - Get news article detail (No auth required)
  - Returns: Complete news article with all details including creator, updater, tags
  - Response: NewsArticleResponse with full information

### Protected Endpoints (Staff & Lecturer - Role 1,2)
- `GET /api/news-articles/my-news` - Get news created by current user
  - **Requires**: Authentication
  - Returns: List of news articles created by the authenticated user
  
- `POST /api/news-articles` - Create news article
  - **Requires**: Authentication (Staff or Lecturer)
  - Request body:
    ```json
    {
      "newsTitle": "string (required)",
      "headline": "string (optional)",
      "newsContent": "string (required)",
      "newsSource": "string (optional)",
      "categoryId": number (required),
      "newsStatus": number (0=Draft, 1=Published),
      "tagIds": [1, 2, 3] (optional)
    }
    ```
  - CreatedDate and ModifiedDate are set automatically
  - CreatedById is extracted from JWT token
  
- `PUT /api/news-articles/{id}` - Update news article
  - **Requires**: Authentication (Staff or Lecturer)
  - Request body: Same as create (UpdateNewsArticleRequest)
  - ModifiedDate is updated automatically
  - UpdatedById is extracted from JWT token
  - Only creator or staff can update
  
- `DELETE /api/news-articles/{id}` - Delete news article (Soft delete)
  - **Requires**: Authentication (Staff or Lecturer)
  - Only creator can delete their own news
  - Sets IsActive = false (soft delete)

## Statistics (Admin Only - Role 0)

### Full Statistics
- `POST /api/news-articles/statistics` - Get complete statistics
  - **Requires**: Admin authentication
  - Request body:
    ```json
    {
      "startDate": "2024-01-01",
      "endDate": "2024-12-31"
    }
    ```
  - Response includes:
    - Overall summary (totalNews, totalPublished, totalDraft, totalAuthors, topCategory)
    - Daily breakdown with category breakdown per day
  - **Note**: Returns all data in one response (may be slower for large date ranges)

### Optimized Statistics Endpoints

- `POST /api/news-articles/statistics/summary` - Get summary statistics only
  - **Requires**: Admin authentication
  - Request body: Same as full statistics
  - Response (StatisticsSummary):
    ```json
    {
      "totalNews": number,
      "totalPublished": number,
      "totalDraft": number,
      "totalAuthors": number,
      "topCategory": {
        "categoryId": number,
        "categoryName": "string",
        "count": number
      }
    }
    ```
  - **Performance**: ~70% lighter payload, loads faster
  - **Use case**: Display summary cards first

- `POST /api/news-articles/statistics/daily-breakdown` - Get daily breakdown only
  - **Requires**: Admin authentication
  - Request body: Same as full statistics
  - Response (List of DailyStatistics):
    ```json
    [
      {
        "date": "2024-11-11",
        "totalNews": number,
        "categoryBreakdown": [
          {
            "categoryId": number,
            "categoryName": "string",
            "count": number
          }
        ]
      }
    ]
    ```
  - **Performance**: Can be loaded separately after summary
  - **Use case**: Display charts and detailed daily breakdown

### Statistics Features
- ✅ Based on CreatedDate (not ModifiedDate)
- ✅ Counts only active news articles (IsActive = true)
- ✅ Includes published (status=1) and draft (status=0) counts
- ✅ Counts distinct authors (CreatedById)
- ✅ Identifies top category by usage count
- ✅ Daily breakdown sorted in descending order (newest first)
- ✅ Category breakdown per day sorted by count (descending)

## Role-Based Access Control

### Admin (Role: 0)
- Full access to Account Management
- Can manage all user accounts (CRUD)

### Staff (Role: 1)
- Manage Categories (CRUD)
- Manage News Articles (CRUD)
- Manage own profile
- View news history created by self

### Lecturer (Role: 2)
- Manage News Articles (CRUD)
- Manage own profile
- View news history created by self

### Guest (No authentication)
- View all news articles
- View news article details
- View all categories

## API Response Format

All endpoints return responses in the following format:

```json
{
  "message": "Success message",
  "statusCode": "200",
  "data": { /* response data */ }
}
```

### Status Codes
- **200**: Success
- **400**: Bad Request
- **401**: Unauthorized (Invalid or missing token)
- **403**: Forbidden (Insufficient permissions)
- **404**: Not Found
- **500**: Internal Server Error

## News Article with Tags

News articles support tags functionality:

### Creating News with Tags
```json
{
  "newsTitle": "Article Title",
  "headline": "Optional headline",
  "newsContent": "Article content",
  "newsSource": "Optional source",
  "categoryId": 1,
  "newsStatus": 1,
  "tagIds": [1, 2, 3]
}
```

### Response includes Tags
```json
{
  "newsArticleId": 1,
  "newsTitle": "Title",
  "tags": [
    { "tagId": 1, "tagName": "Technology" },
    { "tagId": 2, "tagName": "News" }
  ]
}
```

## Tags
- `GET /api/tags` - Get all tags (Public)
  - Returns: List of all active tags
  - Used for tagging news articles

## Key Features Implemented

✅ **Manage Category Information** (Staff - Role 1)
- Staff can create, read, update, and delete categories
- All users can view categories
- Soft delete: Sets IsActive = false

✅ **Manage News Articles with Tags** (Staff & Lecturer - Role 1,2)
- Create news with multiple tags
- Update news and modify tags
- Delete own news articles (soft delete)
- Tags are included in all news responses
- Only active tags are associated with news
- Search functionality across title, headline, content, and source

✅ **Manage Profile** (All authenticated users)
- View own profile via `/api/auth/profile`
- Update profile via `/api/account-management/{id}`
- Change password through profile update

✅ **View News History** (Staff & Lecturer)
- Get news created by current user via `/api/news-articles/my-news`
- Filtered to show only news articles created by the authenticated user
- Includes all article details and associated tags

✅ **Statistics and Reporting** (Admin Only - Role 0)
- Comprehensive statistics by date range
- Summary statistics (lightweight)
- Daily breakdown with category analysis
- Visual charts and trends
- Progressive loading for better performance

## Data Management

### Soft Delete Pattern
The system uses soft delete for all entities:
- **Categories**: `IsActive` flag (default: true)
- **News Articles**: `IsActive` flag (default: true)
- **Tags**: `IsActive` flag (default: true)
- **Accounts**: `IsActive` flag (default: true)

When a record is deleted, it's marked as inactive rather than physically removed from the database.

### Date Tracking
News articles track both creation and modification:
- **CreatedDate**: Set automatically on creation (DateTime.Now)
- **ModifiedDate**: Updated automatically on every update (DateTime.Now)
- **CreatedById**: Extracted from JWT token on creation
- **UpdatedById**: Extracted from JWT token on update

### Tag Management
- Only active tags can be associated with news articles
- When updating news, old tags are removed and new tags are added
- Tags are always returned with news article responses
- Empty tag list is allowed

## Performance Optimizations

### Statistics API
The statistics endpoints are optimized for different use cases:

1. **Summary First Approach**: Load summary quickly to show key metrics
   - Use `/api/news-articles/statistics/summary`
   - ~70% smaller payload
   - Loads in ~0.2s

2. **Progressive Loading**: Load detailed data in background
   - Use `/api/news-articles/statistics/daily-breakdown`
   - Heavier payload with category breakdowns
   - Loads in ~0.5s
   - Doesn't block initial UI rendering

3. **All-in-One**: For smaller date ranges or when all data is needed immediately
   - Use `/api/news-articles/statistics`
   - Returns everything in one call

### Search Optimization
- Search performs case-insensitive matching across multiple fields
- Searches in: NewsTitle, Headline, NewsContent, NewsSource
- Returns only active news articles

## Security Notes

1. **JWT Authentication**: Token expires based on configuration (set in appsettings.json)
2. **Role-Based Authorization**: Endpoints enforce role requirements using `[Authorize(Roles = "...")]`
3. **Ownership Validation**: Users can only delete their own news articles
4. **Token Claims**: AccountId and Role are extracted from JWT for authorization
5. **CORS**: Configured to accept requests from frontend (port 3000)
6. **Password Management**: Passwords can be updated through profile update endpoint

## Error Handling

All endpoints use consistent error response format:

### Common Error Codes
- **400 Bad Request**: Invalid input data or validation errors
- **401 Unauthorized**: Missing or invalid JWT token
- **403 Forbidden**: Valid token but insufficient permissions
- **404 Not Found**: Resource doesn't exist or is inactive
- **500 Internal Server Error**: Server-side error with details in message

### Example Error Response
```json
{
  "message": "News article not found or has been deleted",
  "statusCode": "404",
  "data": null
}
```
