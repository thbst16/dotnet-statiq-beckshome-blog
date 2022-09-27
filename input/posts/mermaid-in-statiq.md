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
<br/><br/>

**Gantt Chart**

Gantt charts for project management can be created via Mermaid as well. Although not suited for large, complex Gantt views, the simplifieid Mermaid charts are great for overall project status and an overview of activities and key dependencies.

<pre data-enlighter-language="md">
gantt
    dateFormat  YYYY-MM-DD
    title       Cloud Migration Mermaid Gantt Charts
    excludes    weekends

    section Data Activities
    Extract and prepare data            :done,    des1, 2021-01-05,2021-01-07
    Validate data integrity             :active,  des2, 2021-01-08, 3d
    Obfuscate non-production data       :         des3, after des2, 5d
    Backup data                         :         des4, after des3, 2d

    section Critical Path Items
    Review migration runbook            :crit, done, 2021-01-05,24h
    Migrate data to cloud               :crit, done, after des1, 2d
    Build and validate app servers      :crit, active, 3d
    Test application in cloud           :crit, 5d
    Pre-production walkthorugh          :crit, 2d
    Receive go-live approval            :crit, 1d
    Cloud go-live                       :milestone, 2021-01-26, 0d

    section Documentation
    Complete compliance documentation   :active, a1, after des1, 3d
    Review Compliance Documentation     :after a1  , 20h
    Revise and Approve Compliance Docs  :doc1, after a1  , 48h
</pre>

<img src="/img/mermaid/gantt.svg"/>