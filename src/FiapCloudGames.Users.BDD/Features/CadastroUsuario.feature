# language: pt-BR
Funcionalidade: Cadastro de Usuário
    Como um novo usuário
    Eu quero me cadastrar na plataforma
    Para que eu possa acessar o sistema e utilizar seus recursos

Contexto:
    Dado que não existe nenhum usuário cadastrado com o email "novo@email.com"

Cenário: Cadastro de usuário com dados válidos
    Quando eu tento cadastrar um usuário com os seguintes dados:
        | Nome          | Email              | Senha       |
        | Maria Silva   | novo@email.com     | Senha@123   |
    Então o cadastro deve ser bem-sucedido
    E o ID do usuário deve ser retornado
    E o usuário deve ser adicionado ao repositório

Cenário: Cadastro com senha sem letra maiúscula
    Quando eu tento cadastrar um usuário com os seguintes dados:
        | Nome          | Email              | Senha       |
        | João Santos   | novo@email.com     | senha@123   |
    Então o cadastro deve falhar com erro "A senha deve conter ao menos uma letra maiúscula."
    E nenhum usuário deve ser adicionado ao repositório

Cenário: Cadastro com senha sem letra minúscula
    Quando eu tento cadastrar um usuário com os seguintes dados:
        | Nome          | Email              | Senha       |
        | João Santos   | novo@email.com     | SENHA@123   |
    Então o cadastro deve falhar com erro "A senha deve conter ao menos uma letra minúscula."
    E nenhum usuário deve ser adicionado ao repositório

Cenário: Cadastro com senha sem número
    Quando eu tento cadastrar um usuário com os seguintes dados:
        | Nome          | Email              | Senha       |
        | João Santos   | novo@email.com     | Senha@abc   |
    Então o cadastro deve falhar com erro "A senha deve conter ao menos um número."
    E nenhum usuário deve ser adicionado ao repositório

Cenário: Cadastro com senha sem caractere especial
    Quando eu tento cadastrar um usuário com os seguintes dados:
        | Nome          | Email              | Senha       |
        | João Santos   | novo@email.com     | Senha123    |
    Então o cadastro deve falhar com erro "A senha deve conter ao menos um caractere especial."
    E nenhum usuário deve ser adicionado ao repositório

Cenário: Cadastro com senha muito curta
    Quando eu tento cadastrar um usuário com os seguintes dados:
        | Nome          | Email              | Senha    |
        | João Santos   | novo@email.com     | Ab1@     |
    Então o cadastro deve falhar com erro "A senha deve ter no mínimo 8 caracteres."
    E nenhum usuário deve ser adicionado ao repositório

Cenário: Cadastro com email já existente
    Dado que já existe um usuário cadastrado com o email "existente@email.com"
    Quando eu tento cadastrar um usuário com os seguintes dados:
        | Nome          | Email                    | Senha       |
        | Pedro Lima    | existente@email.com      | Senha@123   |
    Então o cadastro deve falhar com erro "Email já cadastrado"
    E nenhum usuário deve ser adicionado ao repositório

Cenário: Cadastro com senha vazia
    Quando eu tento cadastrar um usuário com os seguintes dados:
        | Nome          | Email              | Senha |
        | Carlos Souza  | novo@email.com     |       |
    Então o cadastro deve falhar com erro "Senha é obrigatória."
    E nenhum usuário deve ser adicionado ao repositório

Esquema do Cenário: Validação de regras de senha
    Quando eu tento cadastrar um usuário com os seguintes dados:
        | Nome       | Email              | Senha    |
        | Teste User | novo@email.com     | <senha>  |
    Então o cadastro deve falhar com erro "<mensagem_erro>"
    E nenhum usuário deve ser adicionado ao repositório

    Exemplos:
        | senha        | mensagem_erro                                           |
        | abc123       | A senha deve ter no mínimo 8 caracteres.                |
        | ABCDEFGH1!   | A senha deve conter ao menos uma letra minúscula.       |
        | abcdefgh1!   | A senha deve conter ao menos uma letra maiúscula.       |
        | Abcdefgh!    | A senha deve conter ao menos um número.                 |
        | Abcdefgh1    | A senha deve conter ao menos um caractere especial.     |

Cenário: Verificar que usuário é criado com perfil Comum por padrão
    Quando eu tento cadastrar um usuário com os seguintes dados:
        | Nome          | Email              | Senha       |
        | Novo Usuario  | novo@email.com     | Senha@123   |
    Então o cadastro deve ser bem-sucedido
    E o usuário criado deve ter o perfil "Comum"

Cenário: Verificar que a senha é armazenada como hash
    Quando eu tento cadastrar um usuário com os seguintes dados:
        | Nome          | Email              | Senha       |
        | Usuario Hash  | novo@email.com     | Senha@123   |
    Então o cadastro deve ser bem-sucedido
    E a senha deve estar armazenada como hash BCrypt
    E a senha em texto plano não deve estar armazenada