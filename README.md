Shewhart Cards
=============
A Shewhart card is a schedule of changes of process in time. There are tools used to determine if a process is in a state of statistical control.

About
=============
This library provides tools for creation of Shewhart cards, their processing and for data collection, data control, which necessary for work.
It is supposed to use the library for creating of tests of performance. These tests are independent of a computer configuration and state of operation system. With their help it is possible to know when and why there was a failure.

How to work
=============
There are two data collectors in the library:
  1. AlternativeDataCollector – it is works with data which represents fixing existence or absence of a property
  2. QuantitativeDataCollector – it is works with data which represents quantitative characteristic of an object

For both of two data types there are some types of cards.

Alternative data: p-card, np-card, c-card, u-card.
Quantitative data: x-card, r-card. 

For each card it is possible to check if a corresponding test in state of statistical control by calling a method isInStateOfStaticalControl() or in a special (warning) state by calling a method
isInSpecialState().

Example
=============
```
int start;
int duration;
//Select data collector, depending on data type
QuantitativeDataCollector collector = new QuantitativeDataCollector();
//Select subgroups size
int subgroupSize = 4;
//Determine name of data selection and set subgroup size
collector.beginCollecting("PerformanceTest", subgroupSize);
for (int i = 0; i < subgroupSize; i++)
{
    start = Environment.TickCount;
    performanceTestFunc();
    duration = Environment.TickCount - start;
    //Set value
    collector.setValue(duration);
}
//Select card's type and build with data of current selection "PerformanceTest"
IShewhartCard card = collector.getShewhartCard(ShewhartCardsBuilder.QuantitativeCardType.xCard);
//Check method performanceTestFunc() on state of statistical control.
bool isInControl = card.isInStateOfStaticalControl();
```
Required
============
SQLite library in project's directory. It can be taken from this repositoryor from the official site.
