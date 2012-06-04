drop database crawlerdb;

select * from sys.databases;

use crawlerdb;

use crawlerdb

select * from TddDemandRecords

select COUNT(*) from TddDemandRecords where Site='http://careers.stackoverflow.com';
select * from TddDemandRecords where Site='http://careers.stackoverflow.com';
select COUNT(*) from TddDemandRecords where Site='http://careers.stackoverflow.com' and Demand='1';
select Technology, COUNT(*) from TddDemandRecords where Site='http://careers.stackoverflow.com' and Demand='1' group by Technology;

select COUNT(*) from TddDemandRecords where Site='http://rabota.ua';
select * from TddDemandRecords where Site='http://rabota.ua';
select COUNT(*) from TddDemandRecords where Site='http://rabota.ua' and Demand='1';
select Technology, COUNT(*) from TddDemandRecords where Site='http://rabota.ua' and Demand='1' group by Technology;

