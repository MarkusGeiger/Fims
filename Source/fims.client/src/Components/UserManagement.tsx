import { useEffect, useState } from "react";
import { AuthorizedUserData } from "./AuthorizeView";
import "./UserManagement.css"
import RoleManagement from "./RoleManagement";
import DeleteUserButton from "./DeleteUserButton";
import UserEdit from "./UserEdit";
import { User } from "../Types/UserTypes";

function UserManagement() {
  const [users, setUsers] = useState<User[]>();
  const [editUser, setEditUser] = useState<User>();

  useEffect(() => {
    getUserData();
  }, []);

  async function handleEditClick(user: User) {
    console.log("User to be edited: ", user.id);
    setEditUser(user);
  }

  function getUserManagementContent() {
    return <>
      {editUser == undefined || editUser == null ? getUserList() : getUserEditComponent() }
    </>
  }

  function onUserEditSaved(){
    console.log("UserEdit saved in UserManagement");
    
    setEditUser(undefined);
    getUserData();
  }

  function getUserEditComponent(){
    return <>
      <div>
        <UserEdit user={editUser!} onCancel={() => setEditUser(undefined)} onSave={onUserEditSaved}/>
      </div>
    </>
  }

  function getUserList(){
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
        {users?.map(user => {
          const isCurrentUser = AuthorizedUserData() === user.email;
          return (<tr key={user.id} className={isCurrentUser ? "current" : ""}>
            <td>{user.id}</td>
            <td>{user.username}</td>
            <td>{user.email}</td>
            <td>{user.emailConfirmed ? "✅" : "❌"}</td>
            {/* <td>{user.roles.map(r => r + ", ")}</td> */}
            <td>{user.roles}</td>
            <td>
              <button disabled={isCurrentUser} onClick={() => handleEditClick(user)}>edit</button>
              <DeleteUserButton user={user} disabled={isCurrentUser}/>
            </td>
          </tr>);
        }
        )}
      </tbody>
    </table>;
  }

  return (
    <div>
      <h1 id="tabelLabel">Users</h1>
      <p>This component demonstrates fetching data from the server.</p>
      {users === undefined ? getLoadingContent() : getUserManagementContent()}
      { <RoleManagement />}
    </div>
  );

  function getLoadingContent() {
    return <p><em>Loading...</em></p>;
  }

  async function getUserData() {
    const response = await fetch('api/user');
    const data = await response.json();
    console.log("Userdata:", data);

    setUsers(data);
  }

}

export default UserManagement;

