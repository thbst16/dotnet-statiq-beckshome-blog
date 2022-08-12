Title: "Business Logic Reuse - What Color is Your Box?"
Published: 9/19/2006
Tags:
    - Legacy Blog
---
I enjoyed [Harry Piereson’s](http://devhawk.net/blog) well thought-out response to [David Chappel’s entry on SOA and the Reality of Reuse](http://www.davidchappell.com/HTML_email/Opinari_No16_8_06.html). I couldn’t have said it better myself, though that won’t stop me from trying. The way I see it, David brings to light the fact that the emperor has no clothes and then Harry tells you why the emperor is naked. The focus on business context in Harry’s entry really caused me to think about why business logic reuse fails.

The use of the word “context” stirred up in my mind the classic black box, gray box, white box argument. We can expect Java buttons or .NET Windows objects to behave in a low context, black box manner. From contextual business objects, we can at best expect gray box behavior; although white box is much more realistic. I guess in the box world, the opacity of the box is tantamount to the amount of context that can get through.

![What Color Is Your Box](http://s3.beckshome.com/20060919-What-Color-Is-Your-Box.png)

This said, in business object environments that demand high opacity/context, can and should we strive for reuse? I think the answer is still a resounding “yes”. We just need to do it in a realistic manner. So what does this mean? If our experience has shown that neither objects nor Web services are the appropriate level of abstraction for business logic reuse, where do we turn?

In my mind, analysis patterns provide the answer to this question. Analysis patterns are the long lost stepbrother of the popular design patterns. Martin Fowler does a great job of categorizing common analysis patterns in his book of the same name. However, it is Eric Evan’s [Domain Driven Design book](https://www.amazon.com/Domain-Driven-Design-Tackling-Complexity-Software/dp/0321125215) that provides the real insight into the process that will lead to analysis pattern identification.

The true beauty of analysis patterns is that they not only work within their target domain; they have the uncanny ability to shed new light on similar situations across domains. I can well imagine that the case transfer pattlet we created applies not only to the state government business domain for which it was intended, but also fairly well to the transfer of cases in the legal system domain.

In such high context, business rule driven transactions such as case transfers, I would never aspire to build a reusable object or service. Letting the analysis pattern speak for itself, I feel as if I’ve gotten all of the reuse capability I need to at that level. Attempting to create a standard programmatic solution that incorporates all the complexities of the pattlet’s context is likely to cause more problems than it solves. As caretakers of the business logic we need to understand the negative impacts of overengineering a solution and learn to use the right colored box for the job.