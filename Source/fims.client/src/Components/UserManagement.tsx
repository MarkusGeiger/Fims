import { useEffect, useState } from "react";
import { AuthorizedUserData } from "./AuthorizeView";
import "./UserManagement.css"

interface User {
  id: string;
  username: string;
  email: string;
  emailConfirmed: boolean;
  roles: string[];
}

interface Role {
  id: string;
  name: string;
}

function UserManagement() {
  const [users, setUsers] = useState<User[]>();
  const [roles, setRoles] = useState<Role[]>();

  useEffect(() => {
    getUserData();
    getRoleData();
  }, []);

  async function handleDeleteClick(e: React.MouseEvent) {
    const userId = e.currentTarget.getAttribute("data-uid") ?? "";
    console.log("User to be deleted: ", userId);
    const confirmed = confirm(`Do you really want to delete user '${userId}' (${users?.find(u => u.id === userId)?.email})`);
    if (confirmed) {
      await deleteUser(userId);
    }
  }

  return (
    <div>
      <h1 id="tabelLabel">Users</h1>
      <p>This component demonstrates fetching data from the server.</p>
      {users === undefined ? getLoadingContent() : getUserManagementContent(users)}
      {roles === undefined ? getLoadingContent() : getRoleManagementContent(roles)}
    </div>
  );

  function getRoleManagementContent(roles: Role[]){
    return (
    <>
      <p>Available Roles:</p>
      <table>
        <thead>
          <th>ID</th>
          <th>Rolename</th>
        </thead>
        <tbody>
          {roles.map(r => (
            <tr>
              <td>{r.id}</td>
              <td>{r.name}</td>
            </tr>))}
        </tbody>
      </table>
    </>
    );
  }

  function getUserManagementContent(users: User[]) {
    return <table className="table table-striped" aria-labelledby="tabelLabel">
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
        {users.map(user => {
          const isCurrentUser = AuthorizedUserData() === user.email;
          return (<tr key={user.id} className={isCurrentUser ? "current" : ""}>
            <td>{user.id}</td>
            <td>{user.username}</td>
            <td>{user.email}</td>
            <td>{user.emailConfirmed ? "✅" : "❌"}</td>
            {/* <td>{user.roles.map(r => r + ", ")}</td> */}
            <td>{user.roles}</td>
            <td><button disabled={isCurrentUser} data-uid={user.id} onClick={handleDeleteClick}>delete</button></td>
          </tr>);
        }
        )}
      </tbody>
    </table>;
  }

  function getLoadingContent() {
    return <p><em>Loading...</em></p>;
  }

  async function getUserData() {
    const response = await fetch('api/user');
    const data = await response.json();
    console.log("Userdata:", data);

    setUsers(data);
  }

  async function getRoleData() {
    const response = await fetch('api/user/roles');
    const data = await response.json();
    console.log("Roles:", data);

    setRoles(data);
  }

  async function deleteUser(userId: string) {
    const response = await fetch(`api/user?id=${userId}`, { method: "DELETE" })
    console.log("User delete response:", response);

    if (!response.ok) {
      const data = await response.json();
      console.error("Failed to delete user.", data);
    }
    else {
      await getUserData();
    }
  }
}

export default UserManagement;

