pushd input/img/mermaid
../../../node_modules/.bin/mmdc -i flowchart.mmd -o flowchart.svg
../../../node_modules/.bin/mmdc -i sequence.mmd -o sequence.svg
../../../node_modules/.bin/mmdc -i gantt.mmd -o gantt.svg
popd