# Summary 

## Folder Structure:
The folder consists of two.cs projects

1 - A .cs project for the source code
2 - A .cs project for the test code

### Source Code

For this task I followed MVC architecture, you'll find a View, Controller, Model folder within the source code.

The source folder is located under a directory called ``src`` in the root repository.

For this task, I followed an MVC architecture. The model defined in the task was implemented in the ``StoreModel.cs`` under the models folder.
``/src/Wix/Models/StoreModel.cs``

You will find the API implementation in the StoreController under the Controllers folder.
``src/Wix/Controllers/StoreController.cs``

The query parser can be found under the ``QueryLanguage`` folder in the ``Parser.cs`` file
``src/Wix/QueryLanguage/Parser.cs``
That is where the logic for parsing the query expression for the REST API is. It leverages Linq's expressions in a way that might be interesting. :D

### Test Code

The test folder is located under a directory called ``tests`` in the root repository.

You will find the unit tests for the StoreController in the Controllers folder. I mirrored the folder structure of the source folder.
``tests/Wix Unit Tests/Controllers/StoreControllerTests.cs``

The reason I haven't implemented unit tests for the parser is because the parser throws errors that are pretty self explanatory, which means when the StoreControllerTests more or less cover the parser's needs. Adding tests specifically for parsing would be over-engineering in my opinion.


## Notes

You will find the memory cache initialized with the data in the ``Program.cs`` file under the ``src`` folder.
``src/Wix/Program.cs``

All the unit tests when run should pass. :)
