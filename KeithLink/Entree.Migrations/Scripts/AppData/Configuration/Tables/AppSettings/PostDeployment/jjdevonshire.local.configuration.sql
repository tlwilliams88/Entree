-- RabbitMQ Queues

update Configuration.AppSettings
set [Value] = 'devonshire_confirmations'
where [Key] = 'RabbitMQConfirmationQueue'

update Configuration.AppSettings
set [Value] = 'devonshire_orders_history'
where [Key] = 'RabbitMQOrderHistoryQueue'

update Configuration.AppSettings
set [Value] = 'devonshire_orders_updates'
where [Key] = 'RabbitMQOrderUpdateQueue'

-- RabbitMQ Servers

update Configuration.AppSettings
set [Value] = 'qarmq.benekeith.com'
where [Key] = 'RabbitMQAccessServer'

update Configuration.AppSettings
set [Value] = 'qarmq.benekeith.com'
where [Key] = 'RabbitMQConfirmationServer'

update Configuration.AppSettings
set [Value] = 'qarmq.benekeith.com'
where [Key] = 'RabbitMQNotificationServer'

update Configuration.AppSettings
set [Value] = 'qarmq.benekeith.com'
where [Key] = 'RabbitMQOrderServer'

