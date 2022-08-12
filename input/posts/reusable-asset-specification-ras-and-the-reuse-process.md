Title: Reusable Asset Specification (RAS) and the Reuse Process
Published: 12/18/2006
Tags:
    - Legacy Blog
---
In my efforts to understand what drives software reuse or the lack thereof, I’ve been looking for formal reuse standards, processes, and practices. I’ve been examining technology agnostic materials as well as researching the approaches taken by each of the respective major camps – software factories for Microsoft and Model Driven Development (MDD) from the Java community. This post, in particular, is more concerned with technology agnostic materials.

Being a long time believer in Scott Ambler’s Enterprise Unified Process (EUP), the strategic reuse discipline contained in the EUP was the first place I turned for process guidance. The reuse discipline, like the rest of the EUP disciplines, is structured in the same fashion as any other RUP discipline, which makes learning pretty intuitive if you’re familiar with the RUP. The workflow of the strategic reuse discipline is illustrated in the image below. The workflow is pretty straightforward and the harvest, buy, build, evolve approach to the preparation of assets agrees with what I’ve observed in practice. If you’re interested in this process, you can check out a [brief synopsis of the strategic reuse discipline](http://www.enterpriseunifiedprocess.com/essays/strategicReuse.html) or purchases Scott’s book on the EUP.

![EUP Strategic Reuse Discipline](http://s3.beckshome.com/20061218-EUP-Strategic-Reuse-Workflow.jpg)

The [Reusable Asset Specification (RAS)](http://s3.beckshome.com/20061218-OMG-Reusable-Asset-Specification.pdf) is an OMG standard that I have been recommending to colleagues for some while now. In terms of readability, the document is rather dry and academic in nature. In terms of real world applicability, I am aware of several systems that claim to offer RAS compliance. This includes the tool that we use, Logidex, which offers a RAS compliant plugin for Rational XDE. Where the document shines is not in readability or direct applicability but in the way it holistically addresses the capture of metadata about reusable assets. I’ve been recommending that folks read this document with a focus on the UML and XSD diagrams. There is a lot that can be learned about structuring asset metadata that will prove valuable to anyone attempting to classify their software development assets.

![OMG Reusable Asset Specification](http://s3.beckshome.com/20061218-Reusable-Asset-Specification.gif)

As a footnote, the IBM article on reusable assets, recipes and patterns provides a good introduction to the MDD approach, as espouse by one of the major Java vendors. Jack Greenfield’s article on software factories is not all that far removed from the techniques espoused by IBM but, as always, the tooling is quite different.