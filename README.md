# SkillSearch
This project is a demo of a website for handling employees, assigning skills/proficiencies and projects.

The project consists of a backend on C# .NET 8 and a frontend on Angular.

# Backend
## Overview
The backend consists of 2 main projects.

### SkillSearchAPI
This project handles all API calls from the Frontend. Every time there is a change to the data, e.g. a new skill is assigned to an employee, this project is called and handles updating both the SQL database and Algolia.

### SkillSearchHangfireCron
This project handles fetching, updating and storing of all employee data in a SQL database. This is done via a scheduled Cron Job, which is handled by Hangfire. This will fetch data from an external API (In this case the EmployeeAPI project acts as the external API), transform it into the required format and then save it to a SQL database and Algolia which is our Search Engine.

## Software Architecture Diagram
![alt text](https://i.imgur.com/SVpXpHE.jpeg)
