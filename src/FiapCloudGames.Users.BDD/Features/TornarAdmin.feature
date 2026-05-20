# language: pt-BR
Funcionalidade: Promover Usuário a Administrador
    Como um administrador do sistema
    Eu quero promover usuários comuns a administradores
    Para que eles possam gerenciar o sistema

Contexto:
    Dado que estou autenticado como administrador

Cenário: Promover usuário comum a administrador com sucesso
    Dado que existe um usuário comum com os seguintes dados:
        | Nome          | Email                  | Tipo   |
        | João Silva    | joao@email.com         | Comum  |
    Quando eu tento promover o usuário a administrador
    Então a promoção deve ser bem-sucedida
    E o usuário deve ter o perfil "Admin"
    E o usuário deve ser atualizado no repositório

Cenário: Tentar promover usuário que não existe
    Dado que não existe nenhum usuário com o ID "00000000-0000-0000-0000-000000000000"
    Quando eu tento promover o usuário com ID "00000000-0000-0000-0000-000000000000" a administrador
    Então a promoção deve falhar com erro "Usuário não encontrado."
    E nenhum usuário deve ser atualizado no repositório

Cenário: Promover usuário que já é administrador
    Dado que existe um usuário administrador com os seguintes dados:
        | Nome          | Email                  | Tipo   |
        | Maria Admin   | maria@email.com        | Admin  |
    Quando eu tento promover o usuário a administrador
    Então a promoção deve ser bem-sucedida
    E o usuário deve continuar com o perfil "Admin"
    E o usuário deve ser atualizado no repositório

Cenário: Verificar que o repositório é chamado apenas uma vez
    Dado que existe um usuário comum com os seguintes dados:
        | Nome          | Email                  | Tipo   |
        | Carlos Lima   | carlos@email.com       | Comum  |
    Quando eu tento promover o usuário a administrador
    Então a promoção deve ser bem-sucedida
    E o método de atualização do repositório deve ser chamado exatamente uma vez

Cenário: Promover múltiplos usuários em sequência
    Dado que existem os seguintes usuários:
        | Nome          | Email                  | Tipo   |
        | Pedro Santos  | pedro@email.com        | Comum  |
        | Ana Costa     | ana@email.com          | Comum  |
        | Lucas Souza   | lucas@email.com        | Comum  |
    Quando eu promovo todos os usuários a administradores
    Então todas as promoções devem ser bem-sucedidas
    E todos os usuários devem ter o perfil "Admin"

Cenário: Verificar que apenas o tipo de usuário é alterado
    Dado que existe um usuário comum com os seguintes dados:
        | Nome          | Email                    | Tipo   |
        | Roberto Dias  | roberto@email.com        | Comum  |
    Quando eu tento promover o usuário a administrador
    Então a promoção deve ser bem-sucedida
    E o nome do usuário não deve ser alterado
    E o email do usuário não deve ser alterado
    E apenas o tipo deve ser alterado para "Admin"

Esquema do Cenário: Tentar promover usuários com IDs inválidos
    Quando eu tento promover o usuário com ID "<usuarioId>" a administrador
    Então a promoção deve falhar com erro "Usuário não encontrado."
    E nenhum usuário deve ser atualizado no repositório

    Exemplos:
        | usuarioId                              |
        | 00000000-0000-0000-0000-000000000000   |
        | ffffffff-ffff-ffff-ffff-ffffffffffff   |
        | 12345678-1234-1234-1234-123456789012   |

Cenário: Verificar tratamento de erro ao atualizar repositório
    Dado que existe um usuário comum com os seguintes dados:
        | Nome          | Email                  | Tipo   |
        | Erro User     | erro@email.com         | Comum  |
    E que o repositório irá falhar ao atualizar
    Quando eu tento promover o usuário a administrador
    Então a promoção deve falhar
    E uma exceção deve ser lançada

Cenário: Promover usuário e verificar persistência das alterações
    Dado que existe um usuário comum com os seguintes dados:
        | Nome              | Email                      | Tipo   |
        | Usuario Teste     | userteste@email.com        | Comum  |
    Quando eu tento promover o usuário a administrador
    Então a promoção deve ser bem-sucedida
    E as alterações devem ser persistidas no repositório
    E o usuário atualizado deve ter o perfil "Admin"