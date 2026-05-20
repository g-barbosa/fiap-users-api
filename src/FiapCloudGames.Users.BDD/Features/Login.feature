# language: pt-BR
Funcionalidade: Login de Usuário
    Como um usuário cadastrado no sistema
    Eu quero fazer login com minhas credenciais
    Para que eu possa obter um token de acesso e utilizar a API

Contexto:
    Dado que existe um usuário cadastrado com os seguintes dados:
        | Nome          | Email                  | Senha      |
        | João Silva    | joao@email.com         | Senha@123  |

Cenário: Login com credenciais válidas
    Quando eu tento fazer login com o email "joao@email.com" e senha "Senha@123"
    Então o login deve ser bem-sucedido
    E um token JWT deve ser retornado

Cenário: Login com email inexistente
    Quando eu tento fazer login com o email "inexistente@email.com" e senha "Senha@123"
    Então o login deve falhar com erro "Credenciais inválidas."
    E nenhum token deve ser retornado

Cenário: Login com senha incorreta
    Quando eu tento fazer login com o email "joao@email.com" e senha "SenhaErrada@1"
    Então o login deve falhar com erro "Credenciais inválidas."
    E nenhum token deve ser retornado

Cenário: Login de usuário Admin
    Dado que o usuário "joao@email.com" foi promovido a Admin
    Quando eu tento fazer login com o email "joao@email.com" e senha "Senha@123"
    Então o login deve ser bem-sucedido
    E o token gerado deve incluir o perfil "Admin"

Cenário: Login de usuário Comum
    Quando eu tento fazer login com o email "joao@email.com" e senha "Senha@123"
    Então o login deve ser bem-sucedido
    E o token gerado deve incluir o perfil "Comum"

Cenário: Login com email em branco
    Quando eu tento fazer login com o email "" e senha "Senha@123"
    Então o login deve falhar com erro "Credenciais inválidas."
    E nenhum token deve ser retornado

Esquema do Cenário: Tentativas de login com diferentes credenciais inválidas
    Quando eu tento fazer login com o email "<email>" e senha "<senha>"
    Então o login deve falhar com erro "Credenciais inválidas."
    E nenhum token deve ser retornado

    Exemplos:
        | email                     | senha           |
        | invalido@email.com        | Senha@123       |
        | joao@email.com            | SenhaErrada@99  |
        |                           | Senha@123       |
        | joao@email.com            |                 |