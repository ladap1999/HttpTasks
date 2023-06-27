Feature: WalletServiceFeature

	As a user
    In order to create/delete users in user service
    I want to validate API requests/responses

#TCs: 1,2
Scenario: T001_Get not active user balance
	Given New user with firstName 'Lara' and lastName 'Croft' was created
	When Created user gets balance
	Then Status code is '500'
	And Warning message is 'not active user'

#TC: 3
Scenario: T002_Get not existing user balance
	When Get balance for user with invalid Id
	Then Status code is '500'
	And Warning message is 'not active user'

#TC: 43
Scenario: T003_Charge balance for not active user
	Given New user with firstName 'Lara' and lastName 'Croft' was created
	When User charges balance with value '110.1'
	Then Status code is '500'
	And Warning message is 'not active user'

#TC: 44
Scenario: T004_Charge balance for not existing user
	When Charge balance with value '12.3' for user with invalid Id
	Then Status code is '500'
	And Warning message is 'not active user'

#TCs: 45,54
Scenario Outline: T005_Charge balance with warning result for existing user
	Given New user with firstName 'Lara' and lastName 'Croft' was created
	When Set status 'true' to created user
	And User charges balance with value '<FirstAmount>'
	And User charges balance with value '<SecondAmount>'
	Then Status code is '<StatusCode>'
	And Warning message is '<ErrorMessage>'

Examples:
	| FirstAmount | SecondAmount | StatusCode | ErrorMessage                                |
	| 30          | -30.2        | 500        | User has '30.0', you try to charge '-30.2'. |
	| -30         | -30          | 500        | User has '0', you try to charge '-30.0'.    |

#TC: 53
Scenario: T006_Charge minus balance the same as initial value
	Given New user with firstName 'Lara' and lastName 'Croft' was created
	When Set status 'true' to created user
	And User charges balance with value '30'
	And User charges balance with value '-30'
	Then Status code is '200'

#TC: 55
Scenario: T007_Charge minus balance if initial is zero
	Given New user with firstName 'Lara' and lastName 'Croft' was created
	When Set status 'true' to created user
	And User charges balance with value '-30'
	Then Status code is '500'

#TC: 56
Scenario Outline: T008_Charge same balance as initial
	Given New user with firstName Lara and lastName Croft was created
	And Set status 'true' to created user
	When User charges balance with value '<InitialAmount>'
	And User charges balance with value '10'
	And User charges balance with value '<InitialAmount>'
	And Created user gets balance
	Then Status code is '200'
	And Balance of user is '35'

Examples:
	| InitialAmount |
	| 12.5          |

#TC: 15
Scenario: T009_Get balance for active user without transaction
	Given New user with firstName 'Lara' and lastName 'Croft' was created
	When Set status 'true' to created user
	And Created user gets balance 
	Then Status code is '200'

#TC: 16
Scenario: T010_Get balance after revert
	Given New user with firstName 'Lara' and lastName 'Croft' was created
	And Set status 'true' to created user
	And User charges balance with value '10'
	When User reverts transaction 
	And Created user gets balance 
	Then Status code is '200'
	And Balance of user is '0'

	
#TCs: 45, 54, 11, 14
Scenario Outline: T011_Get balance after transaction with next value
	Given New user with firstName 'Lara' and lastName 'Croft' was created
	And Set status 'true' to created user
	When User charges balance with value '<Amount>'
	And Created user gets balance
	Then Status code is '<StatusCode>'
	And Balance of user is '<Balance>'

Examples:
	| Amount       | StatusCode | Balance      |
	| 0.01         | 200        | 0.01         |
	| 9999999.99   | 200        | 9999999.99   |
	| 10000000     | 200        | 10000000     |
	| -0.01        | 200        | -0.01        |
	| -10000000.01 | 200        | -10000000.01 |

#TCs: 48,50
Scenario Outline: T012_Charge with decimal value
	Given New user with firstName 'Lara' and lastName 'Croft' was created
	When Set status 'true' to created user
	And User charges balance with value '<Amount>'
	Then Status code is '<StatusCode>'
	And Warning message is '<ErrorMessage>'
	Examples:
	| Amount | StatusCode | ErrorMessage                                         |
	| 0.001  | 500        | Amount value must have precision 2 numbers after dot |
	| -0.01  | 500        | User has '0', you try to charge '-0.01'.             |
	

#TC: 49
Scenario: T013_Charge with 0_01 Value
	Given New user with firstName 'Lara' and lastName 'Croft' was created
	When Set status 'true' to created user
	And User charges balance with value '0.01'
	Then Status code is '200'

#TCs: 34, 36, 39
Scenario Outline: T014_Revert transaction with border amount
	Given New user with firstName 'Lara' and lastName 'Croft' was created
	And Set status 'true' to created user
	When User charges balance with value '<Amount>'
	And User reverts transaction
	And Created user gets balance
	Then Status code is '<StatusCode>'
	And Balance of user is '<Balance>'

Examples:
	| Amount     | StatusCode | Balance |
	| 0.01       | 200        | 0       |
	| 9999999.99 | 200        | 0       |
	| 10000000   | 200        | 0       |

#TCs: 35, 37
Scenario Outline: T015_Revert transaction with invalid charge
	Given New user with firstName 'Lara' and lastName 'Croft' was created
	And Set status 'true' to created user
	When User charges balance with value '<Amount>'
	And User reverts transaction
	Then Transaction status code is '<StatusCode>'
	
Examples:
	| Amount      | StatusCode |
	| -0.01       | 200        |
	| 10000000.01 | 200        |

#TC: 38
Scenario: T016_Revert reverted transaction
	Given New user with firstName 'Lara' and lastName 'Croft' was created
	And Set status 'true' to created user
	When User charges balance with value '10'
	And User reverts transaction
	And User reverts transaction
	Then Transaction status code is '500'
	And Transaction ends with error message

#TC: 38
Scenario: T017_Revert transaction with wrong id
	When Make reverts transaction with icorrect id
	Then Transaction status code is '404'
	And Transaction ends with error message 'The given key was not present in the dictionary.'


