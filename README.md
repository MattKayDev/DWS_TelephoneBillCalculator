# DWS_TelephoneBillCalculator
App allowing you to create telephone bills for your customers based on the CSV file containing all calls from DWS.


## Things worth mentioning:
- For the current business need the 8 digit number(customer CLI) has extra details in the CSV file so the actualy charge code is in the different column, and for this to work there's an implemented work around which changes the column used for it if the CLI has 8 digits


## Database:
THe database has been created in Access (.mdb) so it can be easily edited by End User with the use of Microsoft Access (at least 2002).
There's not security as it's not required.
Here are some extra useful details about the tables in the database.

-tblCustomerNumber - contain Names of customers and their numbers so the calculator can do a search for the numbers based on the names
![image](https://user-images.githubusercontent.com/20672176/198640878-5597e938-18d1-42f8-8dec-e45d4b59ae93.png)

-tblCustomer - contains which TarrifCards are assigned to each customer
![image](https://user-images.githubusercontent.com/20672176/198640246-79ee3b6e-a71a-4b7b-8eb7-706566719727.png)

tblTarrifCardTemplate - template tarrif card that can be used to create a tarrif card for customer and then assigned in table above
![image](https://user-images.githubusercontent.com/20672176/198641588-58ab4653-9c78-4681-85a3-2fc38172cba9.png)

tblTarrifNames - To display nice name on the bill we've got this table which hold the charge codes and the tarrif names which can be looked up by using the charge code
![image](https://user-images.githubusercontent.com/20672176/198643240-42a6d71c-2c92-47c4-99bb-c56bc3946d22.png)



## What could be improved?
- UI (few changes could be nice, to make it more user friendly)
- The 8 digit work around, could be more dynamic and the data could be seperated better
- Allow End User to make database changes from within the app, this way user won't have to touch the database.

