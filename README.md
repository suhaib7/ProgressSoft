This application was created using .NET Core 8 with the aim of keeping the codebase clean and maintainable.

Features:
Dynamic Functions: I implemented dynamic functions using IUnitOfWork, which integrates with other interfaces to manage the data access layer.

Database Initializer: The app includes an initializer that inserts the Super Admin upon the first run.

SQL Server Database: The project uses SQL Server, where you'll find the test tables and a test file for verification.

Testing Login: For testing purposes, you can log in using the following credentials:
Email: sa
Password: 1

Photo Handling:
As part of the testing process, the user photo part was a bit ambiguous. To address this, I included a feature in the test file that retrieves sample photo URLs from the internet. The FileParser service downloads these photos, assigns them unique IDs, and saves them to a local folder. This ensures that the testing process can proceed without issues related to image files.

Best Practices:
I am still working on incorporating the best coding practices into this application and am always open to feedback and critiques to help me improve.

Backend Focus:
While my focus is primarily on backend development, I am working on improving my Angular skills in order to become a proficient full-stack developer.

Authorization:
I used my previous experience in API authorization for this project. Although it is not strictly required here, I found it helpful in ensuring the application is secure.

Unit Testing:
This is my first time using Unit Testing, and it has been a valuable learning experience. I plan to use Unit Testing in all future applications to ensure better code quality and reliability.

Gratitude:
Thank you for taking the time to review my work. This project has been an excellent opportunity for training and learning new concepts.

Frontend Link:
https://github.com/suhaib7/ProgressSoftFrontend
