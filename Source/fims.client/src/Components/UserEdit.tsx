import { useEffect, useState } from "react";
import { EditableUser, Role, User } from "../Types/UserTypes";


function UserEdit(props: {onCancel: () => void, onSave: () => void, user: User}) {

  const [roles, setRoles] = useState<Role[]>();
  const [user, setUser] = useState<EditableUser>();
  const [pendingChanges, setPendingChanges] = useState<boolean>();

  useEffect(() => {
    getRoleData();
    setUser(new EditableUser(props.user));
  }, []);

  function onRoleChange(e : React.ChangeEvent<HTMLInputElement>){
    if(user === undefined) return;
    console.log(`Role selection has changed to: ${e.target.value}`);
    
    if (e.target.value === "none") {
      user.roles = [];
    }
    else {
      user.roles = [e.target.value]
    }
    setPendingChanges(user.pendingChanges());
  }

  async function onSaveButtonClicked(){
    console.log(`Save button clicked, pending changes: ${user?.pendingChanges()}`, user);
    props.user.username = user!.username;
    props.user.email = user!.email;
    props.user.roles = user!.roles;
    await updateUser();
    props.onSave();
  }

  function onCancelButtonClicked(){
    console.log(`Cancel button clicked, pending changes: ${user?.pendingChanges()}`, user);
    props.onCancel();
  }

  function getRoleSelector(){
    return roles?.map(r => (
    <label key={r.id}>
      <input type="radio"
             name="roles"
             value={r.name}
             onChange={e => onRoleChange(e)}
             defaultChecked={user?.roles.includes(r.name)}
      />
      {r.name}
    </label>
    ));
  }

  function onUsernameChanged(e : React.ChangeEvent<HTMLInputElement>){
    console.log(`Username has changed ${e.target.value}`);
    user!.username = e.target.value;
    setPendingChanges(user?.pendingChanges());
  }

  function onEmailChanged(e : React.ChangeEvent<HTMLInputElement>){
    console.log(`Email has changed to ${e.target.value}`);
    user!.email = e.target.value;
    setPendingChanges(user?.pendingChanges());
  }

  // TODO: Use form
  return <>
    <h2>Edit User {props.user.username}</h2>
    <table>
      <tbody>
        <tr>
          <td>Changes pending</td>
          <td>{pendingChanges ? "yes" : "no"}</td>
        </tr>
        <tr>
          <td>Id</td>
          <td>{user?.id}</td>
        </tr>
        <tr>
          <td>Username</td>
          <td><input type="text" defaultValue={user?.username} onChange={onUsernameChanged}/></td>
        </tr>
        <tr>
          <td>Email</td>
          <td><input type="email" defaultValue={user?.email} onChange={onEmailChanged}/></td>
        </tr>
        <tr>
          <td>Email confirmed</td>
          <td>{user?.emailConfirmed ? "✅" : "❌"}</td>
        </tr>
        <tr>
          <td>Roles</td>
          <td>
            {roles === undefined ? getLoadingContent() : getRoleSelector()}
          </td>
        </tr>
      </tbody>
    </table>
    <button onClick={onSaveButtonClicked}>save</button>
    <button onClick={onCancelButtonClicked}>cancel</button>
  </>;

  function getLoadingContent() {
    return <p><em>Loading...</em></p>;
  }

  async function getRoleData() {
    const response = await fetch('api/user/roles');
    const data = await response.json();
    console.log("Roles:", data);
    
    setRoles([{ id: "", name: "none" } as Role].concat(data));
  }

  async function updateUser() {
    const response = await fetch(`api/user/${user!.id}`, 
      {
        method: "PUT",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify({
            userName: user?.username,
            email: user?.email,
            roles: user?.roles
        }),
      });
    console.log("Update user result", response)
    const data = await response.json();
    console.log("Roles:", data);
  }
}

export default UserEdit;