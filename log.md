# Log

## Assumptions

## Decisions
I have decided to do this exercise to demonstrate the TDD process and design decisions for developing testable code

### The plan
1. Get the test for ProductOne passing
2. Create a new (failing) test for additional requirement
3. Write code to make code pass, refactor if necessary (red-green-refactor) 
4. Repeat 2 and 3 for all ProductOne requirements
5. Repeat for ProductTwo requirements

## Observations

### Potential enhancements
* FluentAssertions library for tests
* Global using for XUnit, Moq etc
* Create factory class for creating processors and inject into Application processor
* Fix all .Net 8 warnings e.g. non-nullable properties

## Todo
* Add dependency injection
* Add better test case for date tests
* Add unit test for DateWrapper
* Add unit tests for ApplicationProcessor 

