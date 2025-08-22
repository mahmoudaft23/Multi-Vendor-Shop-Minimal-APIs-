-- SQLite
INSERT INTO VendorsApproval (Id, Name, CreatedAt, Status, Email, PasswordHash) 
VALUES (
    'v12345', 
    'Vendor Test', 
    datetime('now'), 
    'wait', 
    'vendor@test.com', 
    'hashedpassword123'
);
