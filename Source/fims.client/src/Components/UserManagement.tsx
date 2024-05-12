import {useEffect, useState} from "react";

interface User {
    id: string;
    username: string;
    email: string;
    emailConfirmed: boolean;
    roles: string[];
}

function UserManagement() {
    const [users, setUsers] = useState<User[]>();

    useEffect(() => {
        getUserData();
    }, []);

    async function handleDeleteClick(e: React.MouseEvent){
      const userId = e.currentTarget.getAttribute("data-uid") ?? "";
      console.log("User to be deleted: ", userId);
      const confirmed = confirm(`Do you really want to delete user '${userId}' (${users?.find(u => u.id === userId)?.email})`);
      if(confirmed){
        await deleteUser(userId);
      }
    }
    
    const contents = users === undefined
        ? <p><em>Loading...</em></p>
        : <table className="table table-striped" aria-labelledby="tabelLabel">
            <thead>
            <tr>
                <th>ID</th>
                <th>Username</th>
                <th>Email</th>
                <th>Email confirmed</th>
                <th>Roles</th>
                <th>Actions</th>
            </tr>
            </thead>
            <tbody>
            {users.map(user =>
                <tr key={user.id}>
                    <td>{user.id}</td>
                    <td>{user.username}</td>
                    <td>{user.email}</td>
                    <td>{user.emailConfirmed ? "✅" : "❌"}</td>
                    {/* <td>{user.roles.map(r => r + ", ")}</td> */}
                    <td>{user.roles}</td>
                    <td><button data-uid={user.id} onClick={handleDeleteClick}>delete</button></td>
                </tr>
            )}
            </tbody>
        </table>;

    return (
        <div>
            <h1 id="tabelLabel">Users</h1>
            <p>This component demonstrates fetching data from the server.</p>
            {contents}
        </div>
    );

    async function getUserData() {
        const response = await fetch('api/user');
        const data = await response.json();
        console.log("Userdata:", data);
        
        setUsers(data);
    }

    async function deleteUser(userId: string) {
      const response = await fetch(`api/user?id=${userId}`, {method: "DELETE"})
      console.log("User delete response:", response);
      
      if(!response.ok){
        const data = await response.json();
        console.error("Failed to delete user.", data);
      }
      else {
        await getUserData();
      }
    }
}

export default UserManagement;

