Title: Creating GeoRSS Feeds in C#
Published: 4/12/2007
Tags:
    - Technology Guides
---
The [recent announcement](http://googlemapsapi.blogspot.com/2007/03/kml-and-georss-support-added-to-google.html) that Google will support GeoRSS in addition to KML as a data format for geographic content in Google Maps is long overdue. This is one of those rare areas where Google trailed both Microsoft and Yahoo and did not seem at all willing to budge. Google’s announcement also seals the deal on GeoRSS as the way to syndicate geo-specific data. However, despite the obvious importance of GeoRSS, there is little written material on producing GeoRSS feeds.

![Creating GeoRSS Fees in C#](http://s3.beckshome.com/20070412-Creating-GeoRSS-Feeds-in-CSharp.png)

I promised a brief tutorial on creating a GeoRSS feed with my post on Yahoo’s Tag Maps. More specifically, my post will focus on a boundary update GeoRSS feed. That is, you pass in the maximum and minimum latitudes and longitudes for your map in question and only data about the points that correspond to that particular latitude / longitude box is actually fetched. Obviously, if the user interacts with the map (i.e. panning or zooming), you can use the map’s API and some AJAX’y goodness to make calls to the GeoRSS feed to pick up a new set of points that correspond to the updated map’s boundaries.

The code below represents the most rudimentary and explicit way to construct a GeoRSS feed using ASP.NET and C#. For the purposes of illustration, no third party GeoRSS libraries are used. It’s all basic I/O, streams, and very manual XML construction. Also note a single monolithic call in Page_Load, lack of exception handling and parameterized queries may or may not be the way you want to do things. Try it out though; it does what it’s supposed to do really well. If you have any comments or corrections, just drop me a line.

I plan on posting a follow up in a couple of days with a live GeoRSS feed. I just need to find a nice sized set of simple data that I can load into a database and point my code at. Expect to see this soon.

```cs
1	using System;
2	using System.Configuration;
3	using System.Data;
4	using System.Data.SqlClient;
5	using System.Xml;
6	 
7	public partial class BlogGeoRss : System.Web.UI.Page
8	{
9	protected void Page_Load(object sender, EventArgs e)
10	{
11	this.Response.Clear();
12	this.Response.ContentType = "text/xml";
13	this.Response.ContentEncoding = System.Text.Encoding.UTF8;
14	System.IO.MemoryStream stream = new System.IO.MemoryStream();
15	XmlTextWriter XMLWrite = new XmlTextWriter(stream, System.Text.Encoding.UTF8);
16	 
17	XMLWrite.WriteStartDocument();
18	XMLWrite.WriteWhitespace(Environment.NewLine);
19	XMLWrite.WriteStartElement("rss");
20	XMLWrite.WriteAttributeString("version", "2.0");
21	XMLWrite.WriteAttributeString("xmlns:georss", "http://www.georss.org/georss");
22	XMLWrite.WriteAttributeString("xmlns:gml", "http://www.opengis.net/gml");
23	XMLWrite.WriteWhitespace(Environment.NewLine);
24	 
25	XMLWrite.WriteStartElement("channel");
26	XMLWrite.WriteWhitespace(Environment.NewLine);
27	XMLWrite.WriteElementString("generator", "geoglue.com");
28	XMLWrite.WriteWhitespace(Environment.NewLine);
29	XMLWrite.WriteElementString("title", "GeoGlue GeoRSS Feed");
30	XMLWrite.WriteWhitespace(Environment.NewLine);
31	XMLWrite.WriteElementString("language", "en-us");
32	XMLWrite.WriteWhitespace(Environment.NewLine);
33	 
34	// Pick up the query strings for the latitude / longitude boundaries
35	float UpperBound = 0F, LowerBound = 0F, LeftBound = 0F, RightBound = 0F;
36	try { UpperBound = float.Parse(Request.QueryString["UpperBound"]); }
37	catch (Exception ex) { };
38	try { LowerBound = float.Parse(Request.QueryString["LowerBound"]); }
39	catch (Exception ex) { };
40	try { LeftBound = float.Parse(Request.QueryString["LeftBound"]); }
41	catch (Exception ex) { };
42	try { RightBound = float.Parse(Request.QueryString["RightBound"]); }
43	catch (Exception ex) { };
44	 
45	// Build the item nodes for each of the specific tours
46	SqlCommand cmd = new SqlCommand("SELECT Name, Description, Latitude, Longitude " +
47	"FROM TOUR WHERE (Latitude &amp;lt; @UpperBound) AND (Latitude &amp;gt; @LowerBound) " +
48	"AND (Longitude &amp;gt; @LeftBound) AND (Longitude &amp;lt; @RightBound)",
49	new SqlConnection(ConfigurationManager.ConnectionStrings["GeoGlueDev"].ConnectionString));
50	cmd.CommandType = CommandType.Text;
51	cmd.Parameters.Add(new SqlParameter("@UpperBound", SqlDbType.Float)).Value = UpperBound;
52	cmd.Parameters.Add(new SqlParameter("@LowerBound", SqlDbType.Float)).Value = LowerBound;
53	cmd.Parameters.Add(new SqlParameter("@LeftBound", SqlDbType.Float)).Value = LeftBound;
54	cmd.Parameters.Add(new SqlParameter("@RightBound", SqlDbType.Float)).Value = RightBound;
55	cmd.Connection.Open();
56	SqlDataReader dr = cmd .ExecuteReader();
57	 
58	while (dr.Read())
59	{
60	XMLWrite.WriteStartElement("item");
61	XMLWrite.WriteWhitespace(Environment.NewLine);
62	XMLWrite.WriteElementString("title", (string)dr["Name"]);
63	XMLWrite.WriteWhitespace(Environment.NewLine);
64	XMLWrite.WriteElementString("description", (string)dr["Description"]);
65	XMLWrite.WriteWhitespace(Environment.NewLine);
66	XMLWrite.WriteElementString("georss:point", Convert.ToString(dr["Latitude"]) + " " + Convert.ToString(dr["Longitude"]));
67	XMLWrite.WriteWhitespace(Environment.NewLine);
68	XMLWrite.WriteEndElement();
69	XMLWrite.WriteWhitespace(Environment.NewLine);
70	}
71	cmd.Connection.Close();
72	 
73	XMLWrite.WriteEndElement();
74	XMLWrite.WriteWhitespace(Environment.NewLine);
75	XMLWrite.WriteEndElement();
76	XMLWrite.WriteWhitespace(Environment.NewLine);
77	XMLWrite.WriteEndDocument();
78	XMLWrite.Flush();
79	 
80	System.IO.StreamReader reader;
81	stream.Position = 0;
82	reader = new System.IO.StreamReader(stream);
83	Byte[] bytes = System.Text.Encoding.UTF8.GetBytes(reader.ReadToEnd());
84	this.Response.BinaryWrite(bytes);
85	this.Response.End();
86	}
87	}
```