Title: Mermaid Diagrams in Statiq
Published: 9/26/2022
Tags:
    - Statiq
---
According to the definition on the [Mermaid site](https://mermaid-js.github.io/mermaid/#/README), Mermaid is a JavaScript based diagramming and charting tool that renders Markdown-inspired text definitions to create and modify diagrams dynamically. In short, when creating online Markdown-based documentation or publications, like with GitHub or with static site generators that use Markdown to generate content, you can move from static images to dynamically generated diagrams. Mermaid currently supports diagrams such as flowcharts, sequence diagrams, class diagrams, ER diagrams and Gantt charts with more diagrams being added.

Adding Mermaid diagrams to a Statiq blog, like the one I use, is a pretty straightforward task. Dpvreony has further simplified this task with some great documentation specific to [using Mermaid diagrams with Statiq](https://www.dpvreony.com/articles/mermaid-with-statiq/). I can confirm that his documented approach works well and he identifies a couple of items where the reader can enhance the approach, including processing diagrams in a loop and working all of this into a CI/CD process.

I've provided a couple of examples below. As with everything on this blog, the source can be found on [my Github repo for this blog](https://github.com/thbst16/dotnet-statiq-beckshome-blog).

**Flowchart**

Mermaid enables you to use all the major flowchart shapes and to put toghether some pretty complex mappings, including cross-flow dependencies. The example below is a simplified technology selection process.

<pre data-enlighter-language="md">
flowchart LR
    A[Start] --> B(Collect financial business case details)
    B --> C{Financial benefits to moving to cloud}
    C -->|Yes| D[Select cloud service provider]
    C -->|No| E[Remain on-premise]
</pre>

<img src="/img/mermaid/flowchart.svg"/>
<br/><br/>

**Sequence Diagram**

Mermaid also pulls off sequence diagrams pretty well, as illustrated by the simplified sequence diagram below.

<pre data-enlighter-language="md">
sequenceDiagram
    User-->Application: Update My Address
    Application-->AddressValidationAPI: Validate
    break if address validation API call fails
        Application-->User: show failure
    end
    Application-->AddressService: Update User Address
</pre>

<img src="/img/mermaid/sequence.svg"/>