Title: Creating GeoRSS Feeds in C#
Published: 4/12/2007
Tags:
    - Technology Guides
---
The [recent announcement](http://googlemapsapi.blogspot.com/2007/03/kml-and-georss-support-added-to-google.html) that Google will support GeoRSS in addition to KML as a data format for geographic content in Google Maps is long overdue. This is one of those rare areas where Google trailed both Microsoft and Yahoo and did not seem at all willing to budge. Google’s announcement also seals the deal on GeoRSS as the way to syndicate geo-specific data. However, despite the obvious importance of GeoRSS, there is little written material on producing GeoRSS feeds.

![Creating GeoRSS Fees in C#](https://s3.amazonaws.com/s3.beckshome.com/20070412-Creating-GeoRSS-Feeds-in-CSharp.png)

I promised a brief tutorial on creating a GeoRSS feed with my post on Yahoo’s Tag Maps. More specifically, my post will focus on a boundary update GeoRSS feed. That is, you pass in the maximum and minimum latitudes and longitudes for your map in question and only data about the points that correspond to that particular latitude / longitude box is actually fetched. Obviously, if the user interacts with the map (i.e. panning or zooming), you can use the map’s API and some AJAX’y goodness to make calls to the GeoRSS feed to pick up a new set of points that correspond to the updated map’s boundaries.

The code below represents the most rudimentary and explicit way to construct a GeoRSS feed using ASP.NET and C#. For the purposes of illustration, no third party GeoRSS libraries are used. It’s all basic I/O, streams, and very manual XML construction. Also note a single monolithic call in Page_Load, lack of exception handling and parameterized queries may or may not be the way you want to do things. Try it out though; it does what it’s supposed to do really well. If you have any comments or corrections, just drop me a line.

I plan on posting a follow up in a couple of days with a live GeoRSS feed. I just need to find a nice sized set of simple data that I can load into a database and point my code at. Expect to see this soon.

<pre data-enlighter-language="csharp">
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
	 
public partial class BlogGeoRss : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.Response.Clear();
	    this.Response.ContentType = "text/xml";
    	this.Response.ContentEncoding = System.Text.Encoding.UTF8;
    	System.IO.MemoryStream stream = new System.IO.MemoryStream();
    	XmlTextWriter XMLWrite = new XmlTextWriter(stream, System.Text.Encoding.UTF8);
    
        XMLWrite.WriteStartDocument();
        XMLWrite.WriteWhitespace(Environment.NewLine);
        XMLWrite.WriteStartElement("rss");
	    XMLWrite.WriteAttributeString("version", "2.0");
	    XMLWrite.WriteAttributeString("xmlns:georss", "http://www.georss.org/georss");
    	XMLWrite.WriteAttributeString("xmlns:gml", "http://www.opengis.net/gml");
    	XMLWrite.WriteWhitespace(Environment.NewLine);
	 
    	XMLWrite.WriteStartElement("channel");
    	XMLWrite.WriteWhitespace(Environment.NewLine);
    	XMLWrite.WriteElementString("generator", "geoglue.com");
    	XMLWrite.WriteWhitespace(Environment.NewLine);
    	XMLWrite.WriteElementString("title", "GeoGlue GeoRSS Feed");
    	XMLWrite.WriteWhitespace(Environment.NewLine);
    	XMLWrite.WriteElementString("language", "en-us");
    	XMLWrite.WriteWhitespace(Environment.NewLine);
    
    	// Pick up the query strings for the latitude / longitude boundaries
    	float UpperBound = 0F, LowerBound = 0F, LeftBound = 0F, RightBound = 0F;
    	try { UpperBound = float.Parse(Request.QueryString["UpperBound"]); }
    	    catch (Exception ex) { };
    	try { LowerBound = float.Parse(Request.QueryString["LowerBound"]); }
    	    catch (Exception ex) { };
    	try { LeftBound = float.Parse(Request.QueryString["LeftBound"]); }
    	    catch (Exception ex) { };
    	try { RightBound = float.Parse(Request.QueryString["RightBound"]); }
    	    catch (Exception ex) { };
	 
        // Build the item nodes for each of the specific tours
    	SqlCommand cmd = new SqlCommand("SELECT Name, Description, Latitude, Longitude " +
        "FROM TOUR WHERE (Latitude &amp;lt; @UpperBound) AND (Latitude &amp;gt; @LowerBound) " +
    	"AND (Longitude &amp;gt; @LeftBound) AND (Longitude &amp;lt; @RightBound)",
    	new SqlConnection(ConfigurationManager.ConnectionStrings["GeoGlueDev"].ConnectionString));
    	cmd.CommandType = CommandType.Text;
    	cmd.Parameters.Add(new SqlParameter("@UpperBound", SqlDbType.Float)).Value = UpperBound;
    	cmd.Parameters.Add(new SqlParameter("@LowerBound", SqlDbType.Float)).Value = LowerBound;
    	cmd.Parameters.Add(new SqlParameter("@LeftBound", SqlDbType.Float)).Value = LeftBound;
    	cmd.Parameters.Add(new SqlParameter("@RightBound", SqlDbType.Float)).Value = RightBound;
    	cmd.Connection.Open();
    	SqlDataReader dr = cmd .ExecuteReader();
    
    	while (dr.Read())
    	{
        	XMLWrite.WriteStartElement("item");
        	XMLWrite.WriteWhitespace(Environment.NewLine);
        	XMLWrite.WriteElementString("title", (string)dr["Name"]);
        	XMLWrite.WriteWhitespace(Environment.NewLine);
        	XMLWrite.WriteElementString("description", (string)dr["Description"]);
        	XMLWrite.WriteWhitespace(Environment.NewLine);
        	XMLWrite.WriteElementString("georss:point", Convert.ToString(dr["Latitude"]) + " " + Convert.ToString(dr["Longitude"]));
        	XMLWrite.WriteWhitespace(Environment.NewLine);
            XMLWrite.WriteEndElement();
        	XMLWrite.WriteWhitespace(Environment.NewLine);
    	}
        cmd.Connection.Close();
	 
    	XMLWrite.WriteEndElement();
    	XMLWrite.WriteWhitespace(Environment.NewLine);
    	XMLWrite.WriteEndElement();
    	XMLWrite.WriteWhitespace(Environment.NewLine);
    	XMLWrite.WriteEndDocument();
    	XMLWrite.Flush();
	 
    	System.IO.StreamReader reader;
    	stream.Position = 0;
    	reader = new System.IO.StreamReader(stream);
    	Byte[] bytes = System.Text.Encoding.UTF8.GetBytes(reader.ReadToEnd());
    	this.Response.BinaryWrite(bytes);
    	this.Response.End();
	}
}
</pre>