### PROJETO MS ECOM

- CQRS Pattern com MediatR 
- OCC (Optimistic Concurrency Control) com RowVersion
- Resiliência com Polly { Retry, Timeout, Circuit Breaker, Fallback }
- Logs centralizados com Serilog e Seq para Observabilidade
- Utilização de dockerfile e docker-compose para criação de containers através do WSL
- Uso de solução IAM (Identity and Access Management) com Auth0 para autenticação de usuário
- RabbitMQ com Event Bus e DLQ
- Result Pattern com FluentResult
- API Gateway com YARP - Reverse Proxy
- Uso de uma CPM (Central Package Management)
- Uso do conceito de Read/Write context do CQRS pattern
- Endpoint de /health no Gateway para monitoramento e saúde dos serviços
- Payment usando Webhook para simular notificação de pagamento


-----------------------

### Planos

1. Pensar no Sistema de comunicação renovado do projeto e analisar possiveis dependências circulares
2. Criar um UserReadModel para o serviço de Order que seja atualizado via evento || Verificar uso correto do Write/Read context
3. Padronizar publicações
4. Melhorar o conceito de CQRS aplicado no projeto, evitando que consumers tenham regras de negócios || Colocar as regras de negócio para os handlers via MediatR
5. Adicionar endpoint no Gateway (API Composition)
6. Aprimorar o uso do Saga Pattern Choreography