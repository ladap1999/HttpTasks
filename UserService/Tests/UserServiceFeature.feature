Feature: UserServiceTests

As a user
In order to create/delete users in user service
I want to validate API requests/responses

Scenario Outline: Create users with alternative string credentials
	Given New user with firstName <firstName> and lastName <lastName> was created
	Then Status code is '<statusCode>'

Examples:
	| firstName                          | lastName                           | statusCode |
	|                                    |                                    | 200        |
	| FIRST_NAME_LONG                    | LAST_NAME_LONG                     | 200        |
	| null                               | null                               | 200        |
	| ~!@#$%^&*()-_=+[]\\{} ;':\\",./<>? | ~!@#$%^&*()-_=+[]\\{} ;':\\",./<>? | 200        |
	| j                                  | s                                  | 200        |
	| JOHN                               | SMITH                              | 200        |

Scenario: Create user with digit credentials
	Given New user with digit firstName '22' and digit lastName '1222' was created
	Then Status code is '400'

Scenario: Check that autoincremented ID of the second user more then the first one
	Given Users with firstName and lastName were created
		| firstName | lastName |
		| Tom       | Ford     |
		| Alex      | Slow     |
	Then Second ID is bigger then the first

Scenario: Check ID of next user after deleted the first one
	Given New user with firstName 'Lara' and lastName 'Croft' was created
	When The first user is deleted and new user with firstName 'Anna' and lastName 'Croft' is created
	Then Second ID is bigger then the first
	
Scenario: Delete not existing user
	When User with non existed Id is deleted
	Then Status code is '500'

Scenario: Delete user with not active status
	Given New user with firstName 'Lara' and lastName 'Croft' was created
	When Created user is deleted
	Then Status code is '200'

Scenario: Get status for non existed user
	When Get status of non existed
	Then Status code is '404'

Scenario: Get status of default user
	Given New user with firstName 'Sem' and lastName 'Pitt' was created
	When Get status of created user
	Then Status of user is not active

Scenario: Set and get changed true status of user
	Given New user with firstName Kim and lastName Pitt was created
	When Set status 'true' to created user
	And Set status 'false' to created user
	And Get status of created user
	Then Status code is '200'
	Then Status of user is not active
	
Scenario: Set and get changed false status of user
	Given New user with firstName 'Kim' and lastName 'Pitt' was created
	When Set status 'true' to created user
	And Get status of created user
	Then Status code is '200'
	Then Status of user is active

Scenario: Set status for not existing user
	When Set status for user with invalid Id
	Then Status code is '404'

Scenario: Set true status for active user
	Given New user with firstName 'Harry' and lastName 'Potter' was created
	When Set status 'true' to created user
	And Set status 'true' to created user
	And Get status of created user
	Then Status code is '200'
	Then Status of user is active