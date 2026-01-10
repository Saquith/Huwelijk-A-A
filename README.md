```markdown
# WeddingSite — Static image instructions (exported Google Photos)

This project is configured to use local exported images placed in `wwwroot/images`. The slideshow and top carousel read their image filenames from `wwwroot/appsettings.json` (PhotoSource.Mode = "Local").

Steps to export images from Google Photos and use them locally
1. Open Google Photos and open the album you want to export.
2. Select the images you want (or the whole album).
3. Click the three-dot menu or use the Google Takeout flow and choose "Download" or "Export" to get a ZIP of the selected images.
   - Google Takeout: https://takeout.google.com/ (choose Google Photos).
4. Extract the downloaded ZIP on your computer.
5. Rename the image files if you want a consistent naming scheme (recommended: lowercase, hyphen-separated).
   - Example names used in this project: `wedding-01.jpg`, `wedding-02.jpg`, ...
6. Copy the image files into the Blazor project's `wwwroot/images` folder.

Update the configuration
- Edit `wwwroot/appsettings.json` and list the exact filenames in the `PhotoUrls` array.
- `LocalFolder` should match the folder in `wwwroot` (default: "images").

Example `wwwroot/appsettings.json`:
```json
{
  "PhotoSource": {
    "Mode": "Local",
    "LocalFolder": "images",
    "PhotoUrls": [
      "wedding-01.jpg",
      "wedding-02.jpg",
      "wedding-03.jpg"
    ]
  }
}
```

Tips and best practices
- Resize/optimize images before copying them to `wwwroot/images`.
  - Recommended max dimension: 1920px on the long edge for large photos.
  - For carousel/slideshow, a height around 600–1200px often looks good when cropped with `object-fit: cover`.
  - Use tools like Photoshop, Affinity Photo, or free tools: ImageMagick, Frontend build tools, or https://squoosh.app/.
- Use progressive JPG or optimized WebP for better performance (you can still keep `.jpg` if browser compatibility required).
- Keep filenames short and URL-safe (lowercase, hyphens, no spaces).
- If you have many images, consider lazy-loading or limiting the slideshow to a curated set to avoid a large initial bundle.

Run locally
1. Ensure images are in `wwwroot/images`.
2. Ensure `wwwroot/appsettings.json` lists those filenames.
3. Build and run:
   - dotnet run
4. Open the app (usually https://localhost:5001).

Notes
- Everything stays static — no server-side Google API or OAuth required.
- If you later want to use externally hosted images (CDN or public URLs), change `Mode` to `Urls` and put absolute image URLs in the `PhotoUrls` array.
```