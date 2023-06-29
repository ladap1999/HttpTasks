Feature: UserServiceTests

As a user
In order to create/delete users in user service
I want to validate API requests/responses

#TCs: 1,2,4,5,7
Scenario Outline: T001_Create users with alternative string credentials
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

#TC: 3
Scenario: T002_Create user with digit credentials
	Given New user with digit firstName '22' and digit lastName '1222' was created
	Then Status code is '400'

#TC: 8
Scenario: T003_Check that autoincremented ID of the second user more then the first one
	Given Users with firstName and lastName were created
		| firstName | lastName |
		| Tom       | Ford     |
		| Alex      | Slow     |
	Then Second ID is bigger then the first

#TC: 9
Scenario: T004_Check ID of next user after deleted the first one
	Given New user with firstName 'Lara' and lastName 'Croft' was created
	When The first user is deleted and new user with firstName 'Anna' and lastName 'Croft' is created
	Then Second ID is bigger then the first

#TC: 21	
Scenario: T005_Delete not existing user
	When User with non existed Id is deleted
	Then Status code is '500'

#TC: 22
Scenario: T006_Delete user with not active status
	Given New user with firstName 'Lara' and lastName 'Croft' was created
	When Created user is deleted
	Then Status code is '200'

#TC: 10
Scenario: T007_Get status for non existed user
	When Get status of non existed
	Then Status code is '404'

#TC: 11
Scenario: T008_Get status of default user
	Given New user with firstName 'Sem' and lastName 'Pitt' was created
	When Get status of created user
	Then Status of user is not active

#TCs: 12, 16
Scenario: T009_Set and get changed true status of user
	Given New user with firstName Kim and lastName Pitt was created
	When Set status 'true' to created user
	And Set status 'false' to created user
	And Get status of created user
	Then Status code is '200'
	Then Status of user is not active

#TCs: 13, 15	
Scenario: T010_Set and get changed false status of user
	Given New user with firstName 'Kim' and lastName 'Pitt' was created
	When Set status 'true' to created user
	And Get status of created user
	Then Status code is '200'
	Then Status of user is active

#TC: 14
Scenario: T011_Set status for not existing user
	When Set status for user with invalid Id
	Then Status code is '404'

#TC: 19
Scenario: T012_Set true status for active user
	Given New user with firstName 'Harry' and lastName 'Potter' was created
	When Set status 'true' to created user
	And Set status 'true' to created user
	And Get status of created user
	Then Status code is '200'
	Then Status of user is active