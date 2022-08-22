Title: "Book Review: Ultra-Fast ASP.NET"
Published: 4/1/2010
Tags:
    - .NET Application Architecture
    - Book Reviews
---
I picked up [this gem of a book](https://www.amazon.com/Ultra-Fast-ASP-NET-Build-Ultra-Scalable-Server/dp/1430223839/) when it first came out in eBook format during the PDC. I sent it over to my Kindle and got through the entire book during session downtimes. I planned on being the first to post a review of this book on Amazon but I’ve sat it out too long and will now be the fifth review.

![Ultra-Fast ASP.NET](https://s3.amazonaws.com/s3.beckshome.com/20100401-Ultra-Fast-ASP-Net.jpg)

The first four reviewers did a pretty respectable job of providing and overview of Mr. Kiessig’s qualifications and the book content and have all awarded the book the entirely deserved 5 start rating. Rather than pile on more information about Rick Kiessig or what’s in the book, I’m going to tell you why, as a person who has spent a good amount of time looking at .NET application performance, I recommend this book to every person I work with as mandatory reading:

* Although there are great rules out there for web site optimization and corresponding tools to test these rules (e.g. Yahoo’s Yslow), it’s great to see the client side examples from an ASP.NET specific point of view.
* It’s interesting to see someone who bucks the current trends and provides some real insight on when it’s appropriate to use ORM’s, saying essentially that objects are good but ORM’s might not be the best engine if you’re building a Formula 1 race car.
* Try finding another book that will even touch web gardens, partitioning an application into different AppPools, or using the /3GB switch. Try finding a Microsoft engineer who will talk to you about those items and offer objective guidance.
* The write-up and source code on asynchonous web pages and background worker threads – worth the price of the book alone.
* Creative, out-of-the-box ideas: using SQL Server Express for caching, using BI services to support the web tier of the application, etc. – not the kind of advice you find in your typical MSDN article.

It would be interesting to see how ASP.NET MVC and Silverlight play out performance-wise but alas, these technologies are a bit newer and Mr. Kiessig had to get a book to press. I’d gladly pay for the second edition of this book if it includes a couple of additional chapters that address these technologies. Until then, this is by far the most thorough and pragmatic book on ASP.NET performance to be had on the market. It might be simply an eye-opening read or the book that saves your skin one day. Either way, you won’t regret picking this book up.