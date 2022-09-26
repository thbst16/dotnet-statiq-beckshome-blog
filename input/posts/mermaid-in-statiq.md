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
    title       Adding GANTT diagram functionality to mermaid
    excludes    weekends
    %% (`excludes` accepts specific dates in YYYY-MM-DD format, days of the week ("sunday") or "weekends", but not the word "weekdays".)

    section A section
    Completed task            :done,    des1, 2014-01-06,2014-01-08
    Active task               :active,  des2, 2014-01-09, 3d
    Future task               :         des3, after des2, 5d
    Future task2              :         des4, after des3, 5d

    section Critical tasks
    Completed task in the critical line :crit, done, 2014-01-06,24h
    Implement parser and jison          :crit, done, after des1, 2d
    Create tests for parser             :crit, active, 3d
    Future task in critical line        :crit, 5d
    Create tests for renderer           :2d
    Add to mermaid                      :1d
    Functionality added                 :milestone, 2014-01-25, 0d

    section Documentation
    Describe gantt syntax               :active, a1, after des1, 3d
    Add gantt diagram to demo page      :after a1  , 20h
    Add another diagram to demo page    :doc1, after a1  , 48h

    section Last section
    Describe gantt syntax               :after doc1, 3d
    Add gantt diagram to demo page      :20h
    Add another diagram to demo page    :48h
</pre>

<img src="/img/mermaid/gantt.svg"/>