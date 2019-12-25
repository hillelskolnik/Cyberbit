.\SQLEXPRESS

ConnectionString:
Data Source=.\sqlexpress; Initial Catalog=HillelsTasks; Integrated Security=True

DB Setting scripts:
CREATE DATABASE HillelsTasks;

CREATE TABLE Users(
	Id int NOT NULL PRIMARY KEY,
	Name nvarchar(50) NOT NULL
);

Insert into Users (Id,Name) Values (1, 'Hillel Skolnik');

CREATE TABLE Tasks(
	Id int NOT NULL PRIMARY KEY,
	TaskName nvarchar(50) NOT NULL,
	DueDate datetime NOT NULL,
	UserId int,
	Done bit,
	FOREIGN KEY (UserId) REFERENCES Users(Id)
);

NET Core Web API:
Four endpoints:

1 UserToken: The token is user id related to the user name.

2 Tasks: Get all the tasks associated to the logged in user.

3 InsertTasks: Insert new task.

4 SetTaskDone: Mark task as done;

Client side:
WPF with MVVM architecture:
Perform the actions:
1 Login
2 Add task
3 Mark task as done.
And display all the tasks accordingly.

Thanks
Hillel Skolnik

