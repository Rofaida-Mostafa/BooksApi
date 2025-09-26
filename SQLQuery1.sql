
INSERT INTO Categories (Name)
VALUES 
('Fiction'),
('Science'),
('History');


-- Books
INSERT INTO Books (AuthorName, BookTitle, Description, Price, ImgUrl, ASIN, Stock, CreatedAt, BookStatus, CategoryId)
VALUES
('George Orwell', '1984', 'Dystopian social science fiction novel.', 150.00, 'images/1984.jpg', 'ASIN001', 10, GETDATE(), 1, 1),

('Aldous Huxley', 'Brave New World', 'Classic science fiction novel.', 160.00, 'images/bravenewworld.jpg', 'ASIN002', 8, GETDATE(), 1, 1),

('J.K. Rowling', 'Harry Potter and the Philosopher''s Stone', 'Fantasy novel, first in the Harry Potter series.', 250.00, 'images/harrypotter1.jpg', 'ASIN003', 20, GETDATE(), 1, 1),

('Stephen Hawking', 'A Brief History of Time', 'Popular science book on cosmology.', 200.00, 'images/briefhistory.jpg', 'ASIN004', 15, GETDATE(), 1, 2),

('Carl Sagan', 'Cosmos', 'Explores the universe and science of space.', 180.00, 'images/cosmos.jpg', 'ASIN005', 12, GETDATE(), 1, 2),

('Richard Dawkins', 'The Selfish Gene', 'Book on evolution and biology.', 170.00, 'images/selfishgene.jpg', 'ASIN006', 9, GETDATE(), 1, 2),

('Yuval Noah Harari', 'Sapiens: A Brief History of Humankind', 'History of Homo sapiens.', 220.00, 'images/sapiens.jpg', 'ASIN007', 14, GETDATE(), 1, 3),

('Jared Diamond', 'Guns, Germs, and Steel', 'Factors that shaped human history.', 210.00, 'images/gunsgermssteel.jpg', 'ASIN008', 11, GETDATE(), 1, 3),

('Doris Kearns Goodwin', 'Team of Rivals', 'Biography of Abraham Lincoln.', 190.00, 'images/teamofrivals.jpg', 'ASIN009', 7, GETDATE(), 1, 3),

('Antony Beevor', 'Stalingrad', 'Detailed account of the Battle of Stalingrad.', 175.00, 'images/stalingrad.jpg', 'ASIN010', 10, GETDATE(), 1, 3);
