# Visao conceitual do projeto de microsservicos e-commerce

## Proposito deste documento
Este texto nao descreve apenas como executar o sistema. A ideia e explicar por que este projeto foi desenhado assim, quais conceitos de arquitetura foram aplicados, quais problemas eles resolvem e quais alternativas poderiam ter sido escolhidas.

## 1) Contexto geral da solucao
Este projeto implementa um e-commerce distribuido com servicos independentes para dominios diferentes:
- Gateway de entrada
- Pedidos
- Estoque
- Pagamento
- Usuario
- Autenticacao
- Notificacao assincrona

A proposta arquitetural principal e separar responsabilidades de negocio, reduzir acoplamento entre contextos e permitir evolucao independente de cada servico.

## 2) Conceitos principais usados e o motivo

### 2.1 Microsservicos por contexto de negocio
Cada servico tem responsabilidade clara e ciclo de vida proprio. Isso ajuda em:
- Isolamento de falhas
- Evolucao independente
- Escalabilidade seletiva (escala so o que precisa)
- Organizacao por dominio

Por que faz sentido aqui:
Em e-commerce, fluxos de pedido, pagamento e estoque possuem regras diferentes e mudam em ritmos diferentes. Separar evita um monolito com alto acoplamento.

Trade-off:
- Mais complexidade operacional
- Maior custo de observabilidade e integracao
- Necessidade de pensar em consistencia distribuida

### 2.2 API Gateway (YARP) como ponto unico de entrada
O gateway centraliza roteamento, politicas transversais e simplifica o consumo do front/clientes.

No projeto, o gateway tambem agrega resiliencia com politicas como retry, timeout e circuit breaker.

Por que foi utilizado:
- Evita que cliente conheca todos os endpoints internos
- Permite padronizar cross-cutting concerns
- Facilita evolucao interna sem quebrar consumidor

Trade-off:
- Pode virar gargalo se nao for bem escalado
- Exige cuidado para nao concentrar regra de negocio no gateway

### 2.3 Resiliencia com Polly (Retry, Timeout, Circuit Breaker)
A comunicacao entre componentes distribuidos esta sujeita a falhas transitorias. O uso de resiliencia no gateway evita propagacao imediata dessas falhas para o usuario final.

Motivacao:
- Retry para falhas intermitentes
- Timeout para nao prender recursos indefinidamente
- Circuit breaker para reduzir tempestade de requisicoes quando um downstream esta degradado

Trade-off:
- Configuracao ruim pode piorar latencia
- Retry sem criterio pode aumentar pressao em servicos ja instaveis

### 2.4 Comunicacao orientada a eventos com RabbitMQ
A integracao principal entre servicos de negocio acontece de forma assincrona por eventos de integracao.

Exemplo de fluxo:
- Pedido criado publica evento
- Estoque reage
- Pagamento reage
- Pedido reage ao resultado do pagamento
- Notificacao envia email de forma assincrona

Por que foi utilizado:
- Desacoplamento temporal entre servicos
- Melhor resiliencia para picos
- Fluxo de negocio mais extensivel (novos consumidores sem alterar produtores)

Trade-off:
- Consistencia eventual
- Debug mais complexo
- Necessidade de idempotencia e boa estrategia de reprocessamento

### 2.5 Saga por coreografia
O ciclo de pedido/pagamento/estoque foi modelado como saga coreografada: cada servico reage a eventos e publica novos eventos.

Por que foi utilizado:
- Mantem autonomia dos servicos
- Evita coordenador central unico
- Se encaixa bem em arquitetura orientada a eventos

Trade-off:
- Fluxo fica distribuido, mais dificil de rastrear
- Regras de compensacao ficam espalhadas entre servicos

### 2.6 CQRS (separacao de escrita e leitura)
O projeto separa comandos (alteram estado) de consultas (leitura/composicao). Essa separacao aparece com handlers de comando e query no padrao MediatR.

Por que foi utilizado:
- Claridade de responsabilidades
- Facilita otimizar leitura e escrita de forma independente
- Ajuda a manter camada de aplicacao mais organizada

Trade-off:
- Mais classes e artefatos
- Curva de aprendizado maior para quem entra no projeto

### 2.7 Read models em MongoDB e projecoes
A leitura composta do pedido usa projecoes em banco de leitura. Eventos atualizam modelos de consulta para acesso rapido.

Por que foi utilizado:
- Otimizacao de leitura
- Flexibilidade para modelar resposta orientada a consulta
- Reduz joins distribuidos em tempo de requisicao

Observacao importante:
O projeto usa eventos para projecao de leitura, mas isso nao implica automaticamente Event Sourcing completo (em que o evento e a fonte unica de verdade do estado).

### 2.8 API Composition no Order
A consulta composta do pedido combina dados de mais de um contexto (pedido, usuario e pagamento) para retornar visao consolidada ao cliente.

Por que foi utilizado:
- Experiencia de consumo melhor para o cliente
- Evita multiplas chamadas no front para montar uma unica tela

Trade-off:
- Pode aumentar latencia da consulta
- Exige estrategia de fallback quando um contexto nao responde

### 2.9 MediatR para coordenacao de casos de uso
Comandos e queries sao processados por handlers. Isso reduz acoplamento entre controllers e regras de aplicacao.

Por que foi utilizado:
- Separacao limpa entre entrada HTTP e regra de negocio
- Facilita testes unitarios de casos de uso

### 2.10 FluentValidation e FluentResults
Validacao de entrada e padronizacao de retorno/erros em camada de aplicacao.

Por que foi utilizado:
- Regras de validacao explicitadas
- Fluxo de erro mais previsivel
- Menos excepcao para fluxo esperado de negocio

### 2.11 Concorrencia otimista (rowversion)
No contexto de estoque, ha controle de concorrencia com versao da linha.

Por que foi utilizado:
- Evita sobrescrita silenciosa em atualizacoes simultaneas
- Importante em dominio com disputa de estoque

Trade-off:
- Requer tratamento explicito de conflito no fluxo de atualizacao

### 2.12 Observabilidade (Serilog + Seq + CorrelationId + health checks)
O projeto adota logging estruturado, correlacao entre requisicoes e endpoint de saude.

Por que foi utilizado:
- Facilita troubleshooting em ambiente distribuido
- Melhora rastreabilidade de uma transacao ponta a ponta

### 2.13 Autenticacao centralizada (Auth0 + JWT)
Autenticacao e autorizacao sao tratadas de forma centralizada com emissao/validacao de token.

Por que foi utilizado:
- Menos complexidade de identidade dentro dos servicos de negocio
- Padrao consolidado para APIs distribuidas

### 2.14 Processamento em background (Worker e Hangfire)
O projeto move tarefas nao interativas para processamento assincrono:
- Worker para notificacao por email
- Jobs recorrentes para expiracao/processamento de pagamento

Por que foi utilizado:
- Evita bloquear requisicoes HTTP com tarefas lentas
- Melhora robustez de fluxos que dependem de reprocessamento

### 2.15 Building Blocks compartilhados
Ha uma camada compartilhada para contratos, mensageria, observabilidade, seguranca e utilitarios.

Por que foi utilizado:
- Evita duplicacao entre servicos
- Padroniza comportamentos transversais

Trade-off:
- Precisa governanca para nao virar "monolito compartilhado"
- Mudancas em componentes comuns exigem cuidado de compatibilidade

### 2.16 Central Package Management
Versoes de pacotes ficam centralizadas para toda a solucao.

Por que foi utilizado:
- Governanca de dependencias
- Menor risco de drift de versoes entre servicos

## 3) Conceitos que poderiam ter sido usados (mas nao foram a escolha principal)

### 3.1 Monolito modular
Alternativa: manter tudo em um unico deploy com modulos internos.
Quando faria sentido:
- Time pequeno
- Menor necessidade de escala independente
- Menor custo operacional inicial

Por que nao foi a escolha aqui:
O objetivo do projeto e praticar arquitetura distribuida e seus desafios reais.

### 3.2 Saga por orquestracao (coordenador central)
Alternativa: um orquestrador unico controlando estados da saga (ex.: state machine).
Vantagem:
- Fluxo global mais explicito
Desvantagem:
- Coordenador vira ponto central de dependencia

### 3.3 gRPC para integracao sincrona interna
Alternativa: chamadas internas com contrato fortemente tipado e menor overhead que REST.
Vantagem:
- Performance melhor em cenarios de alta taxa
Desvantagem:
- Mais complexidade para observabilidade e troubleshooting em alguns times

### 3.4 Event Sourcing completo
Alternativa: persistir eventos como fonte primaria do estado em todos os agregados.
Vantagem:
- Auditoria completa temporal
Desvantagem:
- Complexidade alta de modelagem, versionamento e re-hidratacao

### 3.5 Outbox/Inbox pattern formal
Alternativa: garantir entrega confiavel entre persistencia e publicacao de eventos com tabela outbox e consumidores idempotentes com inbox.
Vantagem:
- Maior garantia contra perda/duplicidade em cenarios de falha
Desvantagem:
- Mais infraestrutura e codigo de suporte

### 3.6 Service Mesh
Alternativa: resiliencia e observabilidade na malha de infraestrutura (em vez de parte no codigo).
Vantagem:
- Politicas padronizadas fora da aplicacao
Desvantagem:
- Custo operacional e curva de adocao

### 3.7 Caching distribuido (Redis)
Alternativa: cache para consultas compostas de alta frequencia.
Vantagem:
- Reducao de latencia
Desvantagem:
- Invalidaçao e consistencia mais complexas

### 3.8 Banco unico compartilhado
Alternativa: todos os servicos usando mesmo banco.
Vantagem:
- Simplicidade inicial
Desvantagem:
- Alto acoplamento e perda de autonomia (anti-padrao comum em microsservicos)

## 4) Decisoes de arquitetura em uma frase
- O projeto privilegia autonomia de servicos e desacoplamento.
- Aceita consistencia eventual para ganhar escalabilidade e resiliencia.
- Investe em observabilidade para compensar complexidade distribuida.
- Separa leitura e escrita para clareza e performance de consulta.
- Usa mensageria para fluxos de negocio assincro nos e reativos.

## 5) Riscos naturais da abordagem adotada
- Complexidade operacional maior que um sistema monolitico.
- Necessidade de governanca de contratos de eventos entre servicos.
- Possibilidade de duplicidade de processamento sem idempotencia forte.
- Exigencia de monitoramento constante de filas, retries e jobs.

## 6) Resumo executivo
Este projeto foi desenhado para representar um e-commerce moderno orientado a dominio, com foco em desacoplamento, resiliencia e evolucao independente dos contextos. Em troca, assume a complexidade inerente de sistemas distribuidos. As escolhas (microsservicos, eventos, saga coreografada, CQRS, read models, gateway com resiliencia e observabilidade) mostram uma arquitetura madura para cenarios que exigem escala de negocio e autonomia de times.

Se a prioridade fosse simplicidade operacional imediata, um monolito modular seria um caminho valido. Mas, como laboratorio de arquitetura distribuida e boas praticas de microsservicos em .NET, a estrategia adotada e coerente com o objetivo do projeto.
