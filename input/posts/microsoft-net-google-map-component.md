Title: "Microsoft .NET Google Map Component"
Published: 7/5/2006
Tags:
    - .NET
---
Jacob Reimers’ Google Maps Control has been a genuine blessing for me over the last couple of days. After a lot of prototyping with Google and Yahoo maps, I decided to go with Google maps for GeoGlue and keep Yahoo maps open as an option based upon the development of the APIs as well as any potential licensing or usage constraints. After dealing with the Google APIs directly, and feeling the pain of issues such as the well-known Internet Explorer “Operation Aborted” maps loading issue, I was yearning for an intermediary API that had already thoughtfully addressed some of these issues.

The Google Maps Control does just this and more, and hit the sweet spot of platform combinations that I was dealing with – .NET 2.0 and Google Maps v2.0. The component is well documented and its design well thought out. Its naming convention emulates the Google API naming fairly closely, so when in doubt, most standard Google Map documentation will lead you to the answer of how to address the issue with the control’s API. The control also contains methods to support Google’s newly released geocoding functionality as well as support for Yahoo’s geocoding functionality, which by virtue having been around longer, is more likely to be in use in existing applications.

The component is well maintained and aligns well with the newest releases of the Google API. It is closed source but free for all use (Jacob’s words) although no license is included in the distribution. Best of all, it enables you to remove all the Javascript references in your .NET source code and use pure C# / VB.NET. The sample below is a snippet from GeoGlue that replaced 60 odd lines of Javascript code scattered across several files. In brief, it sets the latitude, longitude, and markers and then adds a number of markers to the map from a data source — all in pure C#.

<pre data-enlighter-language="csharp">
GoogleMap.Latitude = double.Parse(locationResult.Latitude);
GoogleMap.Longitude = double.Parse(locationResult.Longitude);
GoogleMap.Zoom = 1;
while (ProductResults.read())
{
    GoogleMarker gm = new GoogleMarker();
    gm.ID = ProductResults["Title"].ToString();
    gm.Latitude = double.Parse(ProductResults.["Latitude"]);
    gm.Longitude = double.Parse(ProductResults.["Longitude"]);
    gm.MarkerText = "<b>" + ProductResults.["Title"].ToString() + "</b><br\>" + ProductResults.["Description"].ToString();
    GoogleMap.Markers.Add(gm);
}
</pre>