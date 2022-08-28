Title: SOA Practices
Published: 10/27/2006
Tags:
    - Legacy Blog
---
Over the last several months, I’ve really been trying to get my arms around SOA and develop a meaningful opinion and knowledge base on this so often used, even more often abused, and ever-more-frequently maligned three letter acronym (TLA). Along the way, I’ve discovered a couple of great resources that have helped shape my thinking and hone my implementation skills on the topic:

* [Prentice Hall’s Service-Oriented Computing Series](http://www.soabooks.com/) – Right now this series consists of just two books by Thomas Erl, “[Service-Oriented Architecture: Concepts, Technology, and Design](https://www.amazon.com/Service-Oriented-Architecture-SOA-Concepts-Technology/dp/0131858580/)” and “[Service-Oriented Architecture: A Field Guide to Integrating XML and Web Services](https://www.amazon.com/Service-Oriented-Architecture-Guide-Integrating-Services/dp/0131428985)”. These two books, however, are tantamount to the old and new testament; literally comprising the SOA bible. The series is also slated to be expanded into a more complete collection, along the lines of Addison-Wesley’s Martin Fowler and Kent Beck signature series.

    ![Prentice Hall - SOA Book 1](https://s3.amazonaws.com/s3.beckshome.com/20061027-Prentice-Hall-SOA-1.jpg)![Prentice Hall - SOA Book 2](http://s3.beckshome.com/20061027-Prentice-Hall-SOA-2.jpg)

    Thomas’s books have helped me to understand how my traditional proxy and wrapper-based viewpoints fit into service design and how I might be able to improve the robustness of SOA interfaces built using these patterns. These books also reinforced my positive experiences with contract-driven design and have rekindled my interest in XML schema definitions, which I haven’t used extensively for years.

* [Enterprise Service Orientation Maturity Model (ESOMM)](https://docs.microsoft.com/en-us/previous-versions/bb245664(v=msdn.10)?redirectedfrom=MSDN) – “Maturity model… uh oh, here comes the heavy handed approach to SOA”, you might be thinking. In my opinion, however, this is the most dense (that is, succinct and knowledge rich) piece of material about SOA that has been published to date and a must read for anyone looking to role out SOA to their enterprise. The ESOMM defines 4 layers, 3 perspectives, and 27 capabilities required to support a SOA (see diagram below). The maturity levels are based upon SEI’s Capability Maturity Model (CMM), but the similarities pretty much end there. As with the core CMM, think of this as a roadmap towards evaluating and improving your organization’s SOA capabilities – not as a report card.

    ![Enterprise Service Orientation Maturity Model](https://s3.amazonaws.com/s3.beckshome.com/20061027-ESOMM-Large.gif)

* **Service-Oriented Analysis and Design (SOAD)** – This is a nifty article which seeks to bridge the gaps between the object oriented and business process oriented design and modeling and the requirements of modeling for an SOA. The article does a great job of walking through traditional approaches that most people are familiar with and then adding SOAD-specific elements to the design. The article concludes with a short case study that includes traditional models such as a business process workflow, class diagram, and state diagram and then augmenting this with a service breakdown model and a rather interesting business interaction model (see diagram below) that integrates SOA specific concerns into a more traditional UML sequence diagram.

    ![Service Oriented Analysis and Design](https://s3.amazonaws.com/s3.beckshome.com/20061027-Service-Oriented-Analysis-and-Design-Large.gif)

* Java and .NET Specific Implementation Materials – Jeffrey Hasan’s book [Expert Service-Oriented Architecture in C#](https://www.amazon.com/Expert-Service-Oriented-Architecture-2005-Second/dp/159059701X) is by far the best text in the .NET realm. Jeffrey starts with a very solid approach of WSDL and XSD contract driven design and then gradually introduces the new WS-* standards, integrating them one-by-one. The original book covers WSE 2.0 with his newest text covering WSE 3.0. [SOA Integration Using Java EE 5 Web Services](https://www.amazon.com/SOA-Using-Java-Web-Services/dp/0130449687/) is the best text that I’ve found from a Java vantage point. I like the fact that this book starts out with REST (Representational State Transfer) type services to show how things look before all of the standards-compliant overhead is added. Good coverage is provided for JAX-WS 2.0, JAXB, and JSR-compliant packaging. The book is not yet available on the open market but the work in progress is available as a “Rough Cut” book through O’Reilly’s Safari.

    ![Expert SOA in C#](https://s3.amazonaws.com/s3.beckshome.com/20061027-Expert-SOA-CSharp.jpg)![SOA Integration Using Java EE 5 Web Services](https://s3.amazonaws.com/s3.beckshome.com/20061027-SOA-Integration-Using-Java-EE-5-Web-Services.jpg)

* **BPEL-Specific Materials** – BPEL specific implementation details haven’t seemed to make it into any published books yet. The vendors offer a good deal of online materials in this area. To learn it this way though, you’re going to have to commit yourself to a particular implementation. Good materials can be found in the following places:
    * JBoss – JBoss jBPM
    * Sun – NetBeans Enterprise Pack
    * Oracle – BPEL Process Manager
    * NET – Windows Workflow Foundation